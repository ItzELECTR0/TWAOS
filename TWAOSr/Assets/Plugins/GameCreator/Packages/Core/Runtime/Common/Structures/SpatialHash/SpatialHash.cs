using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class SpatialHash
    {
        private const int INITIAL_CAPACITY = 256;
        private const int MIN_USE_SPATIAL_HASH = 32;
        
        private const int MAX_DIMENSION = 16;
        private const int MIN_DIMENSION = 4;
        
        private static readonly float3[] Bounds = new float3[4];
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly Dictionary<int, ISpatialHash> m_Instances = 
            new Dictionary<int, ISpatialHash>();

        private NativeHashSet<int> m_DynamicInstances =
            new NativeHashSet<int>(INITIAL_CAPACITY, Allocator.Persistent);

        private NativeHashMap<int, Record> m_Records =
            new NativeHashMap<int, Record>(INITIAL_CAPACITY, Allocator.Persistent);
        
        private NativeHashMap<HashKey, int> m_HashCensus =
            new NativeHashMap<HashKey, int>(INITIAL_CAPACITY, Allocator.Persistent);
        
        private NativeParallelMultiHashMap<HashKey, int> m_HashData =
            new NativeParallelMultiHashMap<HashKey, int>(INITIAL_CAPACITY, Allocator.Persistent);
        
        private NativeList<Candidate> m_Candidates = 
            new NativeList<Candidate>(INITIAL_CAPACITY, Allocator.Persistent);
        
        private int m_UpdateFrame = -1;
        
        // INITIALIZE METHODS: --------------------------------------------------------------------

        public SpatialHash()
        {
            ApplicationManager.EventExit -= this.OnExit;
            ApplicationManager.EventExit += this.OnExit;
        }

        private void OnExit()
        {
            ApplicationManager.EventExit -= this.OnExit;
            
            this.m_Instances.Clear();
            
            this.m_DynamicInstances.Dispose();
            this.m_Records.Dispose();
            
            this.m_HashCensus.Dispose();
            this.m_HashData.Dispose();
            this.m_Candidates.Dispose();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void Insert(ISpatialHash item)
        {
            if (item == null) return;
            if (ApplicationManager.IsExiting) return;
            if (this.m_Instances.ContainsKey(item.UniqueCode)) return;

            int uniqueCode = item.UniqueCode;
            float3 position = item.Position;
            bool isDynamic = ((Component) item).gameObject.isStatic == false;
            
            this.m_Instances.Add(uniqueCode, item);
            this.m_Records.Add(uniqueCode, new Record(uniqueCode, position, isDynamic));
            if (isDynamic) this.m_DynamicInstances.Add(uniqueCode);

            int dimension = MAX_DIMENSION;
            while (dimension >= MIN_DIMENSION)
            {
                int3 hash = HashKey.Hash(dimension, position);
                HashKey hashKey = new HashKey(dimension, hash);

                this.m_HashData.Add(hashKey, uniqueCode);

                if (this.m_HashCensus.ContainsKey(hashKey)) this.m_HashCensus[hashKey] += 1;
                else this.m_HashCensus.Add(hashKey, 1);
                
                dimension >>= 1;
            }
        }

        public void Remove(ISpatialHash item)
        {
            if (item == null) return;
            if (ApplicationManager.IsExiting) return;
            
            int uniqueCode = item.UniqueCode;
            
            if (this.m_Records.TryGetValue(uniqueCode, out Record record) == false) return;
            int dimension = MAX_DIMENSION;
            
            while (dimension >= MIN_DIMENSION)
            {
                int3 hash = HashKey.Hash(dimension, record.Position);
                HashKey hashKey = new HashKey(dimension, hash);

                this.m_HashData.Remove(hashKey, uniqueCode);
                this.m_HashCensus[hashKey] -= 1;
                
                dimension >>= 1;
            }

            if (this.m_Records[uniqueCode].IsDynamic)
            {
                this.m_DynamicInstances.Remove(uniqueCode);
            }
            
            this.m_Instances.Remove(uniqueCode);
            this.m_Records.Remove(uniqueCode);
        }

        public bool Contains(ISpatialHash item)
        {
            if (ApplicationManager.IsExiting) return false;

            int uniqueCode = item.UniqueCode;
            return this.m_Records.ContainsKey(uniqueCode);
        }

        public void Find(Vector3 point, float radius, List<ISpatialHash> results, ISpatialHash except = null)
        {
            if (ApplicationManager.IsExiting) return;
            
            m_Candidates.Clear();
            results.Clear();
            
            if (this.m_Records.Count < MIN_USE_SPATIAL_HASH)
            {
                foreach (KeyValuePair<int, ISpatialHash> entry in this.m_Instances)
                {
                    if (entry.Value == null || entry.Value == except) continue;
                    
                    float distance = math.distance(point, entry.Value.Position);
                    if (distance > radius) continue;
                    
                    m_Candidates.Add(new Candidate(entry.Key, distance));
                }
            }
            else
            {
                this.UpdateRecords();
                
                Bounds[0] = new float3(point.x - radius, point.y + radius, point.z - radius);
                Bounds[1] = new float3(point.x + radius, point.y + radius, point.z - radius);
                Bounds[2] = new float3(point.x - radius, point.y - radius, point.z - radius);
                Bounds[3] = new float3(point.x - radius, point.y - radius, point.z + radius);
            
                this.GetCandidates(MAX_DIMENSION, point, radius, except?.UniqueCode ?? 0);
            }
            
            this.m_Candidates.Sort();
            
            if (results.Capacity < m_Candidates.Length)
            {
                results.Capacity = m_Candidates.Length;
            }
            
            foreach (Candidate candidate in m_Candidates)
            {
                int uniqueCode = candidate.UniqueCode;
                results.Add(this.m_Instances[uniqueCode]);
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void GetCandidates(int dimension, float3 point, float radius, int exceptUniqueCode)
        {
            int3 cellA = HashKey.Hash(dimension, Bounds[0]); // (-1,  1,  1)
            int3 cellB = HashKey.Hash(dimension, Bounds[1]); // ( 1,  1,  1)
            int3 cellC = HashKey.Hash(dimension, Bounds[2]); // (-1, -1,  1)
            int3 cellD = HashKey.Hash(dimension, Bounds[3]); // (-1,  1, -1)
            
            for (int x = cellA.x; x <= cellB.x; ++x)
            {
                for (int y = cellC.y; y <= cellA.y; ++y)
                {
                    for (int z = cellC.z; z <= cellD.z; ++z)
                    {
                        int3 cellHash = new int3(x, y, z);
                        HashKey hashKey = new HashKey(dimension, cellHash);

                        if (!this.m_HashCensus.TryGetValue(hashKey, out int census) || census == 0)
                        {
                            continue;
                        }

                        if (census < MIN_USE_SPATIAL_HASH || dimension <= MIN_DIMENSION)
                        {
                            foreach (int uniqueCode in this.m_HashData.GetValuesForKey(hashKey))
                            {
                                if (uniqueCode == exceptUniqueCode) continue;

                                float3 instancePosition = this.m_Records[uniqueCode].Position;
                                float distance = math.distance(instancePosition, point);
                                if (distance > radius) continue;
                                
                                m_Candidates.Add(new Candidate(uniqueCode, distance));
                            }
                        }
                        else
                        {
                            this.GetCandidates(dimension >> 1, point, radius, exceptUniqueCode);
                        }
                    }
                }
            }
        }

        private void UpdateRecords()
        {
            if (this.m_UpdateFrame == Time.frameCount) return;

            foreach (int uniqueCode in this.m_DynamicInstances)
            {
                float3 currentPosition = this.m_Records[uniqueCode].Position;
                float3 newPosition = ((Component) this.m_Instances[uniqueCode]).transform.position;
                
                if (currentPosition.Equals(newPosition)) continue;
                
                int dimension = MAX_DIMENSION;
                while (dimension >= MIN_DIMENSION)
                {
                    int3 curHash = HashKey.Hash(dimension, currentPosition);
                    int3 newHash = HashKey.Hash(dimension, newPosition);
                    
                    bool differentHash = 
                        curHash.x != newHash.x ||
                        curHash.y != newHash.y ||
                        curHash.z != newHash.z;
                    
                    if (differentHash)
                    {
                        HashKey curHashKey = new HashKey(dimension, curHash);
                        HashKey newHashKey = new HashKey(dimension, newHash);
                        
                        this.m_HashData.Remove(curHashKey, uniqueCode);
                        this.m_HashCensus[curHashKey] -= 1;
                        
                        this.m_HashData.Add(newHashKey, uniqueCode);
                        if (this.m_HashCensus.ContainsKey(newHashKey)) this.m_HashCensus[newHashKey] += 1;
                        else this.m_HashCensus.Add(newHashKey, 1);
                    }
                    
                    dimension >>= 1;
                }
            
                bool isDynamic = this.m_Records[uniqueCode].IsDynamic;
                this.m_Records[uniqueCode] = new Record(
                    uniqueCode,
                    newPosition,
                    isDynamic
                );
            }
            
            this.m_UpdateFrame = Time.frameCount;
        }
    }
}