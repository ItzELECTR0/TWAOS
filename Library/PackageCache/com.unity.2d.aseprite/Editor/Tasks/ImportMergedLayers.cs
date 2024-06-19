using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.U2D.Aseprite
{
    internal static class ImportMergedLayers
    {
        public static void Import(string assetName, ref List<Layer> layers, out List<NativeArray<Color32>> cellBuffers, out List<int> cellWidth, out List<int> cellHeight)
        {
            var cellsPerFrame = GetAllCellsPerFrame(in layers);
            var mergedCells = MergeCells(in cellsPerFrame, assetName);

            cellBuffers = new List<NativeArray<Color32>>();
            cellWidth = new List<int>();
            cellHeight = new List<int>();

            for (var i = 0; i < mergedCells.Count; ++i)
            {
                var width = mergedCells[i].cellRect.width;
                var height = mergedCells[i].cellRect.height;
                if (width == 0 || height == 0)
                    continue;

                cellBuffers.Add(mergedCells[i].image);
                cellWidth.Add(width);
                cellHeight.Add(height);
            }

            UpdateLayerList(mergedCells, assetName, ref layers);
        }

        static Dictionary<int, List<Cell>> GetAllCellsPerFrame(in List<Layer> layers)
        {
            var cellsPerFrame = new Dictionary<int, List<Cell>>();
            for (var i = 0; i < layers.Count; ++i)
            {
                var cells = layers[i].cells;
                for (var m = 0; m < cells.Count; ++m)
                {
                    var cell = cells[m];
                    var width = cell.cellRect.width;
                    var height = cell.cellRect.height;
                    if (width == 0 || height == 0)
                        continue;

                    if (cellsPerFrame.ContainsKey(cell.frameIndex))
                        cellsPerFrame[cell.frameIndex].Add(cell);
                    else
                        cellsPerFrame.Add(cell.frameIndex, new List<Cell>() { cell });
                }

                var linkedCells = layers[i].linkedCells;
                for (var m = 0; m < linkedCells.Count; ++m)
                {
                    var frameIndex = linkedCells[m].frameIndex;
                    var linkedToFrame = linkedCells[m].linkedToFrame;

                    var cellIndex = cells.FindIndex(x => x.frameIndex == linkedToFrame);
                    Assert.AreNotEqual(-1, cellIndex, $"Linked Cell: {frameIndex} is linked to cell: {linkedToFrame}, which cannot be found.");

                    var cell = cells[cellIndex];

                    var width = cell.cellRect.width;
                    var height = cell.cellRect.height;
                    if (width == 0 || height == 0)
                        continue;

                    if (cellsPerFrame.ContainsKey(frameIndex))
                        cellsPerFrame[frameIndex].Add(cell);
                    else
                        cellsPerFrame.Add(frameIndex, new List<Cell>() { cell });
                }
            }

            return cellsPerFrame;
        }

        static List<Cell> MergeCells(in Dictionary<int, List<Cell>> cellsPerFrame, string assetName)
        {
            var mergedCells = new List<Cell>(cellsPerFrame.Count);
            foreach (var (frameIndex, cells) in cellsPerFrame)
            {
                unsafe
                {
                    var count = cells.Count;

                    var textures = new NativeArray<IntPtr>(count, Allocator.Persistent);
                    var cellRects = new NativeArray<RectInt>(count, Allocator.Persistent);
                    var cellBlendModes = new NativeArray<BlendModes>(count, Allocator.Persistent);

                    for (var i = 0; i < cells.Count; ++i)
                    {
                        textures[i] = (IntPtr)cells[i].image.GetUnsafePtr();
                        cellRects[i] = cells[i].cellRect;
                        cellBlendModes[i] = cells[i].blendMode;
                    }

                    TextureTasks.MergeTextures(in textures, in cellRects, in cellBlendModes, out var output);
                    var mergedCell = new Cell()
                    {
                        cellRect = output.rect,
                        image = output.image,
                        frameIndex = frameIndex,
                        name = ImportUtilities.GetCellName(assetName, frameIndex, cellsPerFrame.Count),
                        spriteId = GUID.Generate()
                    };
                    mergedCells.Add(mergedCell);

                    textures.Dispose();
                    cellRects.Dispose();
                    cellBlendModes.Dispose();
                }
            }

            return mergedCells;
        }

        static void UpdateLayerList(in List<Cell> cells, string assetName, ref List<Layer> layers)
        {
            layers.Clear();
            var flattenLayer = new Layer()
            {
                cells = cells,
                index = 0,
                name = assetName
            };
            flattenLayer.guid = Layer.GenerateGuid(flattenLayer);
            layers.Add(flattenLayer);
        }
    }
}
