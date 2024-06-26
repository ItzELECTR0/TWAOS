#if GPU_INSTANCER
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdAnimator : GPUIAnimatorWorkflow
    {
        public Vector4 currentClipStartTimes;
        public GPUIAnimationClipData[] newAnimationClipData;
        public Vector4 newClipStartTimes;
        public float[] currentClipSpeeds;
        
        public GPUICrowdTransition transition;
        public int transitionIndex;
        public bool isInTransition { get { return transitionIndex >= 0; } }

        public GPUICrowdAnimator()
        {
            ResetAnimator();
        }

        #region Public Methods
        public void ResetAnimator()
        {
            currentAnimationClipData = new GPUIAnimationClipData[4];
            currentAnimationClipDataWeights = Vector4.zero;
            currentClipStartTimes = Vector4.zero;
            newAnimationClipData = new GPUIAnimationClipData[4];
            newClipStartTimes = Vector4.zero;
            currentClipSpeeds = new float[4];
            transitionIndex = -1;
        }

        public void StartAnimation(GPUICrowdRuntimeData runtimeData, int arrayIndex, AnimationClip animationClip, float startTime = -1.0f, float speed = 1.0f, float transitionTime = 0)
        {
            if (runtimeData.animationClipDataDict.TryGetValue(animationClip.GetHashCode(), out GPUIAnimationClipData clipData))
                StartAnimation(runtimeData, arrayIndex, clipData, startTime, speed, transitionTime);
            else
                Debug.LogError("Animation clip was not baked. Can not start animation: " + animationClip.name);
        }

        public void StartAnimation(GPUICrowdRuntimeData runtimeData, int arrayIndex, GPUIAnimationClipData clipData, float startTime = -1.0f, float speed = 1.0f, float transitionTime = 0)
        {
            float currentTime = Time.time;

            bool hasTransition = transitionTime > 0;

            int previousClipIndex = GetCurrentClipIndex(clipData);

            if (speed <= 0)
                speed = 0.000001f;

            if (startTime >= 0)
                newClipStartTimes.x = currentTime - startTime;
            else if (previousClipIndex >= 0)
            {
                if (speed == currentClipSpeeds[previousClipIndex])
                    newClipStartTimes.x = currentClipStartTimes[previousClipIndex];
                else
                    newClipStartTimes.x = GetSpeedRelativeStartTime(currentTime, currentClipStartTimes[previousClipIndex], currentClipSpeeds[previousClipIndex], clipData.length, speed);
            }
            else
                newClipStartTimes.x = currentTime;
            newClipStartTimes.y = 0;
            newClipStartTimes.z = 0;
            newClipStartTimes.w = 0;

            if (hasTransition)
            {
                currentAnimationClipData[3] = currentAnimationClipData[2];
                currentAnimationClipData[2] = currentAnimationClipData[1];
                currentAnimationClipData[1] = currentAnimationClipData[0];
                currentAnimationClipData[0] = clipData;

                currentAnimationClipDataWeights.w = currentAnimationClipDataWeights.z;
                currentAnimationClipDataWeights.z = currentAnimationClipDataWeights.y;
                currentAnimationClipDataWeights.y = currentAnimationClipDataWeights.x;
                currentAnimationClipDataWeights.x = 0.01f;

                currentClipSpeeds[3] = currentClipSpeeds[2];
                currentClipSpeeds[2] = currentClipSpeeds[1];
                currentClipSpeeds[1] = currentClipSpeeds[0];
                currentClipSpeeds[0] = speed;

                newClipStartTimes[3] = currentClipStartTimes[2];
                newClipStartTimes[2] = currentClipStartTimes[1];
                newClipStartTimes[1] = currentClipStartTimes[0];

                if (transition == null)
                    transition = new GPUICrowdTransition();
                transition.SetData(arrayIndex, currentTime, transitionTime, activeClipCount, currentAnimationClipDataWeights, new Vector4(1, 0, 0, 0), 1);
                if (transitionIndex < 0)
                {
                    transitionIndex = runtimeData.transitioningAnimators.Count;
                    runtimeData.transitioningAnimators.Add(this);
                }

                activeClipCount += 1;
                if (activeClipCount > 4)
                    activeClipCount = 4;
            }
            else
            {
                currentAnimationClipData[3] = default;
                currentAnimationClipData[2] = default;
                currentAnimationClipData[1] = default;
                currentAnimationClipData[0] = clipData;

                currentAnimationClipDataWeights.w = 0;
                currentAnimationClipDataWeights.z = 0;
                currentAnimationClipDataWeights.y = 0;
                currentAnimationClipDataWeights.x = 1;

                currentClipSpeeds[0] = speed;

                activeClipCount = 1;
            }


            BlendAnimations(runtimeData, arrayIndex, hasTransition);
        }

        public void StartBlend(GPUICrowdRuntimeData runtimeData, int arrayIndex, Vector4 animationWeights, AnimationClip animationClip1, AnimationClip animationClip2, AnimationClip animationClip3 = null, AnimationClip animationClip4 = null, float[] animationTimes = null, float[] animationSpeeds = null, float transitionTime = 0)
        {
            StartBlend(runtimeData, arrayIndex, animationWeights, GetClipData(runtimeData, animationClip1), GetClipData(runtimeData, animationClip2), GetClipData(runtimeData, animationClip3), GetClipData(runtimeData, animationClip4), animationTimes, animationSpeeds, transitionTime);
        }

        public void StartBlend(GPUICrowdRuntimeData runtimeData, int arrayIndex, Vector4 animationWeights, GPUIAnimationClipData clipData1, GPUIAnimationClipData clipData2, GPUIAnimationClipData clipData3, GPUIAnimationClipData clipData4, float[] animationTimes = null, float[] animationSpeeds = null, float transitionTime = 0)
        {
            float currentTime = Time.time;
            bool hasTransition = transitionTime > 0;
            int previousClipCount = activeClipCount;
            activeClipCount = 2;
            if (clipData3.clipFrameCount > 0)
                activeClipCount++;
            if (clipData4.clipFrameCount > 0)
                activeClipCount++;
            if (activeClipCount == 4)
                hasTransition = false;
            if (hasTransition)
            {
                if (previousClipCount + activeClipCount > 4)
                    previousClipCount -= (previousClipCount + activeClipCount) % 4;
                activeClipCount += previousClipCount;
            }

            if (clipData4.clipFrameCount > 0)
                newAnimationClipData[3] = clipData4;
            else if (hasTransition && activeClipCount == 4)
                newAnimationClipData[3] = currentAnimationClipData[previousClipCount == 1 ? 0 : 1];
            else
                newAnimationClipData[3] = default;

            if (clipData3.clipFrameCount > 0)
                newAnimationClipData[2] = clipData3;
            else if (hasTransition)
                newAnimationClipData[2] = currentAnimationClipData[0];
            else
                newAnimationClipData[2] = default;

            newAnimationClipData[0] = clipData1;
            newAnimationClipData[1] = clipData2;

            for (int i = 0; i < 4; i++)
            {
                if (animationTimes != null && animationTimes.Length > i)
                    newClipStartTimes[i] = animationTimes[i];
                else
                    newClipStartTimes[i] = GetClipTime(newAnimationClipData[i]);

                // Updated with speeds below
            }

            for (int i = 0; i < 4; i++)
            {
                currentAnimationClipData[i] = newAnimationClipData[i];
            }

            if (hasTransition)
            {
                Vector4 newWeights = Vector4.zero;
                for (int i = 0; i < activeClipCount; i++)
                {
                    if (i < activeClipCount - previousClipCount)
                        newWeights[i] = 0.01f;
                    else
                        newWeights[i] = currentAnimationClipDataWeights[i - activeClipCount + previousClipCount];
                }
                currentAnimationClipDataWeights = newWeights;

                if (transition == null)
                    transition = new GPUICrowdTransition();
                transition.SetData(arrayIndex, currentTime, transitionTime, previousClipCount, newWeights, animationWeights, activeClipCount - previousClipCount);
                if (transitionIndex < 0)
                {
                    transitionIndex = runtimeData.transitioningAnimators.Count;
                    runtimeData.transitioningAnimators.Add(this);
                }
            }
            else
                currentAnimationClipDataWeights = animationWeights;

            int speedCount = 0;
            if (animationSpeeds != null)
                speedCount = animationSpeeds.Length;
            for (int i = 0; i < 4; i++)
            {
                if (i < speedCount)
                {
                    if (animationSpeeds[i] <= 0)
                        currentClipSpeeds[i] = 0.000001f;
                    else
                        currentClipSpeeds[i] = animationSpeeds[i];
                }
                else
                    currentClipSpeeds[i] = 1;
                newClipStartTimes[i] = currentTime - (newClipStartTimes[i] / currentClipSpeeds[i]);
            }

            BlendAnimations(runtimeData, arrayIndex, hasTransition);
        }

        public void SetAnimationSpeed(GPUICrowdRuntimeData runtimeData, int arrayIndex, float animationSpeed)
        {
            runtimeData.dependentJob.Complete();
            int crowdAnimIndex = arrayIndex * 4;
            for (int i = 0; i < activeClipCount; i++)
            {
                Vector4 animatorData = runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i];
                if (animationSpeed <= 0)
                    animationSpeed = 0.000001f;
                currentClipStartTimes[i] = GetSpeedRelativeStartTime(Time.time, currentClipStartTimes[i], currentClipSpeeds[i], currentAnimationClipData[i].length, animationSpeed);
                animatorData.w = currentClipStartTimes[i];
                currentClipSpeeds[i] = animationSpeed;
                animatorData.z = animationSpeed;

                runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i] = animatorData;
            }
            //runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData, crowdAnimIndex, crowdAnimIndex, activeClipCount);
            runtimeData.crowdAnimatorDataModified = true;
        }

        public void SetAnimationSpeeds(GPUICrowdRuntimeData runtimeData, int arrayIndex, float[] animationSpeeds)
        {
            runtimeData.dependentJob.Complete();
            int speedCount = 0;
            if (animationSpeeds != null)
                speedCount = animationSpeeds.Length;
            int crowdAnimIndex = arrayIndex * 4;
            for (int i = 0; i < activeClipCount; i++)
            {
                Vector4 animatorData = runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i];
                if (animationSpeeds[i] <= 0)
                    animationSpeeds[i] = 0.000001f;
                currentClipStartTimes[i] = GetSpeedRelativeStartTime(Time.time, currentClipStartTimes[i], currentClipSpeeds[i], currentAnimationClipData[i].length, animationSpeeds[i]);
                animatorData.w = currentClipStartTimes[i];
                if (i < speedCount)
                    currentClipSpeeds[i] = animationSpeeds[i];
                else
                    currentClipSpeeds[i] = 1;
                animatorData.z = currentClipSpeeds[i];

                runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i] = animatorData;
            }
            //runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData, crowdAnimIndex, crowdAnimIndex, activeClipCount);
            runtimeData.crowdAnimatorDataModified = true;
        }


        public void SetAnimationWeights(GPUICrowdRuntimeData runtimeData, int arrayIndex, Vector4 animationWeights)
        {
            runtimeData.dependentJob.Complete();
            int animationWeightIndex = arrayIndex * 2 + 1;

            // set weights
            currentAnimationClipDataWeights = animationWeights;
            runtimeData.animationData[animationWeightIndex] = currentAnimationClipDataWeights;

            //runtimeData.animationDataBuffer.SetData(runtimeData.animationData, animationWeightIndex, animationWeightIndex, 1);
            runtimeData.animationDataModified = true;
        }

        public void RemoveFromTransitioningAnimatorsSwapBack(GPUICrowdRuntimeData runtimeData)
        {
            int lastIndex = runtimeData.transitioningAnimators.Count - 1;
            if (lastIndex != transitionIndex)
            {
                GPUICrowdAnimator lastAnimator = runtimeData.transitioningAnimators[lastIndex];
                lastAnimator.transitionIndex = transitionIndex;
                runtimeData.transitioningAnimators[transitionIndex] = lastAnimator;
            }

            transitionIndex = -1;
            runtimeData.transitioningAnimators.RemoveAt(lastIndex);
        }

        public bool ApplyTransition(GPUICrowdRuntimeData runtimeData, float currentTime)
        {
            if (transitionIndex < 0)
                return false;

            int animationWeightIndex = transition.arrayIndex * 2 + 1;

            if (currentTime > transition.startTime + transition.totalTime)
            {
                activeClipCount = transition.endActiveClipCount;
                currentAnimationClipDataWeights = transition.endWeights;
                RemoveFromTransitioningAnimatorsSwapBack(runtimeData);
            }
            else
            {
                currentAnimationClipDataWeights = Vector4.Lerp(transition.startWeights, transition.endWeights, (currentTime - transition.startTime) / transition.totalTime);
            }

            // set weights
            runtimeData.animationData[animationWeightIndex] = currentAnimationClipDataWeights;

            //runtimeData.animationDataBuffer.SetData(runtimeData.animationData, animationWeightIndex, animationWeightIndex, 1);
            return transitionIndex >= 0;
        }

        public void ApplyAnimationEvents(GPUICrowdRuntimeData runtimeData, GPUICrowdPrefab crowdInstance, float currentTime, float deltaTime)
        {
            int count = activeClipCount;
            for (int i = 0; i < count; i++)
            {
                GPUIAnimationClipData clipData = currentAnimationClipData[i];
                List<GPUIAnimationEvent> gpuiAnimationEvents;
                if (runtimeData.eventDict.TryGetValue(clipData.clipIndex, out gpuiAnimationEvents))
                {
                    //float clipTotalTime = GetClipTotalTime(i, currentTime);
                    //if (clipData.IsLoopDisabled() && clipTotalTime > clipData.length)
                    //    continue;

                    int currentClipFrame = Mathf.CeilToInt(GetClipFrame(i, currentTime, clipData.length, clipData.clipFrameCount, clipData.IsLoopDisabled()));
                    int previousClipFrame = Mathf.CeilToInt(GetClipFrame(i, currentTime - deltaTime, clipData.length, clipData.clipFrameCount, clipData.IsLoopDisabled()));
                    foreach (GPUIAnimationEvent gpuiAnimationEvent in gpuiAnimationEvents)
                    {
                        if (gpuiAnimationEvent.eventFrame <= currentClipFrame && (gpuiAnimationEvent.eventFrame > previousClipFrame || previousClipFrame > currentClipFrame))
                            gpuiAnimationEvent.Invoke(crowdInstance, gpuiAnimationEvent.floatParam, gpuiAnimationEvent.intParam, gpuiAnimationEvent.stringParam);
                    }
                }
            }
        }

        public float GetClipTime(GPUICrowdRuntimeData runtimeData, AnimationClip animationClip)
        {
            int animationKey = animationClip.GetHashCode();
            if (!runtimeData.animationClipDataDict.ContainsKey(animationKey))
            {
                Debug.LogError("Animation clip was not baked. Can not get time for animation: " + animationClip.name);
                return 0;
            }
            GPUIAnimationClipData clipData = runtimeData.animationClipDataDict[animationKey];

            return GetClipTime(clipData);
        }

        public float GetClipTime(GPUIAnimationClipData clipData)
        {
            if (clipData.clipFrameCount == 0)
                return 0;
            int clipIndex = GetCurrentClipIndex(clipData);
            if (clipIndex >= 0)
            {
                return GetClipTotalTime(clipIndex, Time.time) % clipData.length;
            }
            return 0;
        }


        public float GetClipTotalTime(int clipIndex, float currentTime)
        {
            return (currentTime - currentClipStartTimes[clipIndex]) * currentClipSpeeds[clipIndex];
        }

        public float GetClipFrame(int clipIndex, float currentTime, float clipLength, int clipFrameCount, bool isLoopDisabled)
        {
            float clipTotalTime = GetClipTotalTime(clipIndex, currentTime);
            if (isLoopDisabled && clipTotalTime > clipLength)
                return clipFrameCount - 1;
            else
                return ((clipTotalTime / clipLength) % 1.0f) * (clipFrameCount - 1);
        }

        public static float GetClipFrame(float clipTotalTime, float clipLength, int clipFrameCount, bool isLoopDisabled)
        {
            if (isLoopDisabled && clipTotalTime > clipLength)
                return clipFrameCount - 1;
            else
                return ((clipTotalTime / clipLength) % 1.0f) * (clipFrameCount - 1);
        }

        public void SetClipTime(GPUICrowdRuntimeData runtimeData, int arrayIndex, AnimationClip animationClip, float time)
        {
            if (runtimeData.animationClipDataDict.TryGetValue(animationClip.GetHashCode(), out GPUIAnimationClipData clipData))
                SetClipTime(runtimeData, arrayIndex, clipData, time);
            else
            {
                Debug.LogError("Animation clip was not baked. Can not set time for animation: " + animationClip.name);
                return;
            }
        }

        public void SetClipTime(GPUICrowdRuntimeData runtimeData, int arrayIndex, GPUIAnimationClipData clipData, float time)
        {
            for (int i = 0; i < currentAnimationClipData.Length; i++)
            {
                if (currentAnimationClipData[i].clipIndex == clipData.clipIndex)
                {
                    newClipStartTimes[i] = Time.time - (time / (currentClipSpeeds[i] > 0 ? currentClipSpeeds[i] : 1));
                    BlendAnimations(runtimeData, arrayIndex);
                    return;
                }
            }
        }

        public void UpdateIndex(GPUICrowdRuntimeData runtimeData, int arrayIndex)
        {
            newClipStartTimes = currentClipStartTimes;
            BlendAnimations(runtimeData, arrayIndex, transitionIndex >= 0);
        }

        public GPUIAnimationClipData GetClipData(GPUICrowdRuntimeData runtimeData, AnimationClip animationClip)
        {
            if (animationClip == null)
                return default;
            runtimeData.animationClipDataDict.TryGetValue(animationClip.GetHashCode(), out GPUIAnimationClipData clipData);
            return clipData;
        }
        #endregion Public Methods

        #region Private Methods
        private void BlendAnimations(GPUICrowdRuntimeData runtimeData, int arrayIndex, bool hasTransition = false)
        {
            if (!hasTransition && transitionIndex >= 0)
            {
                RemoveFromTransitioningAnimatorsSwapBack(runtimeData);
            }

            int animationIndex = arrayIndex * 2;
            int crowdAnimIndex = arrayIndex * 4;
            currentClipStartTimes = newClipStartTimes;

            GPUIAnimationClipData clipData;
            Vector4 animationFrames = Vector4.zero;
            for (int i = 0; i < 4; i++)
            {
                if (i < activeClipCount)
                {
                    clipData = currentAnimationClipData[i];

                    // set min-max frames and speed
                    runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i] = new Vector4(
                        clipData.clipStartFrame,
                        // If loop is disabled send max frame as negative (trick to conserve memory)
                        clipData.IsLoopDisabled() ? -(clipData.clipStartFrame + clipData.clipFrameCount - 1) : clipData.clipStartFrame + clipData.clipFrameCount - 1,
                        currentClipSpeeds[i],
                        currentClipStartTimes[i]);

                    // set current clip frame
                    animationFrames[i] = clipData.clipStartFrame + GetClipFrame(i, Time.time, clipData.length, clipData.clipFrameCount, clipData.IsLoopDisabled());
                }
                else
                {
                    runtimeData.crowdAnimatorControllerData[crowdAnimIndex + i] = Vector4.zero;
                    animationFrames[i] = -1.0f;
                }
            }
            runtimeData.animationData[animationIndex] = animationFrames;
            // set weights
            runtimeData.animationData[animationIndex + 1] = currentAnimationClipDataWeights;

            // set data to buffers
            //runtimeData.animationDataBuffer.SetData(runtimeData.animationData, animationIndex, animationIndex, 2);
            //runtimeData.crowdAnimatorControllerBuffer.SetData(runtimeData.crowdAnimatorControllerData, crowdAnimIndex, crowdAnimIndex, activeClipCount);
            runtimeData.animationDataModified = true;
            runtimeData.crowdAnimatorDataModified = true;

            newClipStartTimes = Vector4.zero;
            runtimeData.disableFrameLerp = true;
        }

        private float GetSpeedRelativeStartTime(float currentTime, float previousStartTime, float previousSpeed, float clipLength, float newSpeed)
        {
            return currentTime - (((currentTime - previousStartTime) * previousSpeed) % clipLength) / newSpeed;
        }

        private int GetCurrentClipIndex(GPUIAnimationClipData clipData)
        {
            for (int i = 0; i < currentAnimationClipData.Length; i++)
            {
                if (currentAnimationClipData[i].clipFrameCount > 0 && currentAnimationClipData[i].clipIndex == clipData.clipIndex)
                    return i;
            }
            return -1;
        }
        #endregion Private Methods
    }

#if GPUI_BURST
    [Unity.Burst.BurstCompile]
#endif
    struct ApplyCrowdAnimatorRootMotionJob : IJobParallelForTransform
    {
        [ReadOnly] public float currentTime;
        [ReadOnly] public float lerpAmount;
        [ReadOnly] public NativeArray<Vector4> animationData;   // index: 0 x -> frameNo1, y -> frameNo2, z -> frameNo3, w -> frameNo4
                                                                // index: 1 x -> weight1, y -> weight2, z -> weight3, w -> weight4
        [ReadOnly] public NativeArray<Vector4> crowdAnimatorControllerData; // 0 to 4: x ->  minFrame, y -> maxFrame (negative if not looping), z -> speed, w -> startTime
        [ReadOnly] public NativeArray<GPUIAnimationClipData> clipDatas;
        [ReadOnly] public NativeArray<GPUIRootMotion> rootMotions;

        public void Execute(int index, TransformAccess transformAccess)
        {
            Matrix4x4 currentMatrix = transformAccess.localToWorldMatrix;
            Vector3 scale = currentMatrix.lossyScale;
            bool matrixChanged = false;
            Vector4 currentWeights = animationData[index * 2 + 1];
            for (int i = 0; i < 4; i++)
            {
                Vector4 currentAnimatorData = crowdAnimatorControllerData[index * 4 + i];
                if (currentAnimatorData.y != 0)
                {
                    GPUIAnimationClipData clipData = default;
                    for (int c = 0; c < clipDatas.Length; c++)
                    {
                        if (currentAnimatorData.x == clipDatas[c].clipStartFrame)
                        {
                            clipData = clipDatas[c];
                            break;
                        }
                    }

                    if (!clipData.HasRootMotion())
                        continue;
                    float clipTotalTime = (currentTime - currentAnimatorData.w) * currentAnimatorData.z;
                    if (clipData.IsLoopDisabled() && clipTotalTime > clipData.length)
                        continue;

                    int clipFrame = Mathf.CeilToInt(GPUICrowdAnimator.GetClipFrame(clipTotalTime, clipData.length, clipData.clipFrameCount, clipData.IsLoopDisabled()));
                    GPUIRootMotion motionData = rootMotions[clipData.clipStartFrame + clipFrame];
                    if (!motionData.HasMotion())
                        continue;

                    matrixChanged = true;

                    float lerp = lerpAmount * currentAnimatorData.z * currentWeights[i];
                    if (motionData.IsPositionOnly())
                    {
                        Vector3 motionPosition = motionData.motionMatrix.GetColumn(3) * lerp;
                        motionPosition = transformAccess.rotation * motionPosition;
                        for (int l = 0; l < 3; l++)
                        {
                            currentMatrix[l, 3] += motionPosition[l] * scale[l];
                        }
                    }
                    else
                    {
                        Matrix4x4 motionMatrix = currentMatrix * motionData.motionMatrix;

                        for (int l = 0; l < 16; l++)
                        {
                            float a = currentMatrix[l];
                            currentMatrix[l] = a + ((motionMatrix[l] - a) * lerp);
                        }
                    }
                }
            }
            if (matrixChanged)
            {
                transformAccess.position = currentMatrix.GetColumn(3);
                transformAccess.rotation = currentMatrix.rotation;
            }
        }
    }
}
#endif //GPU_INSTANCER