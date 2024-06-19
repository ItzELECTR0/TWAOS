using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
    internal static class AnimationClipGeneration
    {
        const string k_RootName = "Root";

        public static AnimationClip[] Generate(string assetName,
            Sprite[] sprites,
            AsepriteFile file,
            List<Layer> layers,
            List<Frame> frames,
            List<Tag> tags,
            Dictionary<int, GameObject> layerIdToGameObject)
        {
            var noOfFrames = file.noOfFrames;
            if (tags.Count == 0)
            {
                var tag = new Tag();
                tag.name = assetName + "_Clip";
                tag.fromFrame = 0;
                tag.toFrame = noOfFrames;

                tags.Add(tag);
            }

            var layersWithDisabledRenderer = new HashSet<Layer>();
            for (var i = 0; i < layers.Count; ++i)
            {
                if (DoesLayerDisableRenderer(layers[i], tags))
                    layersWithDisabledRenderer.Add(layers[i]);
            }

            var clips = new List<AnimationClip>(tags.Count);
            var animationNames = new HashSet<string>(tags.Count);
            for (var i = 0; i < tags.Count; ++i)
            {
                var clipName = tags[i].name;
                if (animationNames.Contains(clipName))
                {
                    var nameIndex = 0;
                    while (animationNames.Contains(clipName))
                        clipName = $"{tags[i].name}_{nameIndex++}";

                    Debug.LogWarning($"The animation clip name {tags[i].name} is already in use. Renaming to {clipName}.");
                }

                clips.Add(CreateClip(tags[i], clipName, layers, layersWithDisabledRenderer, frames, sprites, layerIdToGameObject));
                animationNames.Add(clipName);
            }

            return clips.ToArray();
        }

        static bool DoesLayerDisableRenderer(Layer layer, IReadOnlyList<Tag> tags)
        {
            if (layer.layerType != LayerTypes.Normal)
                return false;

            var cells = layer.cells;
            var linkedCells = layer.linkedCells;

            for (var i = 0; i < tags.Count; ++i)
            {
                var tag = tags[i];
                for (var frameIndex = tag.fromFrame; frameIndex < tag.toFrame; ++frameIndex)
                {
                    var foundCell = false;
                    foreach (var cell in cells)
                    {
                        if (cell.frameIndex != frameIndex)
                            continue;
                        foundCell = true;
                        break;
                    }

                    if (foundCell)
                        continue;

                    foreach (var cell in linkedCells)
                    {
                        if (cell.frameIndex != frameIndex)
                            continue;
                        foundCell = true;
                        break;
                    }

                    if (!foundCell)
                        return true;
                }
            }
            return false;
        }

        static AnimationClip CreateClip(Tag tag, string clipName, List<Layer> layers, HashSet<Layer> layersWithDisabledRenderer, IReadOnlyList<Frame> frames, Sprite[] sprites, Dictionary<int, GameObject> layerIdToGameObject)
        {
            var animationClip = new AnimationClip()
            {
                name = clipName,
                frameRate = 100f
            };

            var clipSettings = new AnimationClipSettings();
            clipSettings.loopTime = tag.isRepeating;
            AnimationUtility.SetAnimationClipSettings(animationClip, clipSettings);

            for (var i = 0; i < layers.Count; ++i)
            {
                var layer = layers[i];
                if (layer.layerType != LayerTypes.Normal)
                    continue;

                var layerGo = layerIdToGameObject[layer.index];
                if (layerGo.GetComponent<SpriteRenderer>() == null)
                    continue;

                var doesLayerDisableRenderer = layersWithDisabledRenderer.Contains(layer);
                var layerTransform = layerGo.transform;
                var spriteKeyframes = new List<ObjectReferenceKeyframe>();

                var cells = layer.cells;
                var activeFrames = AddCellsToClip(cells, in tag, in sprites, frames, ref spriteKeyframes);

                var linkedCells = layer.linkedCells;
                activeFrames.UnionWith(AddLinkedCellsToClip(linkedCells, in cells, in tag, in sprites, frames, ref spriteKeyframes));

                spriteKeyframes.Sort((x, y) => x.time.CompareTo(y.time));
                DuplicateLastFrame(ref spriteKeyframes, frames[tag.toFrame - 1], animationClip.frameRate);

                var path = GetTransformPath(layerTransform);
                var spriteBinding = EditorCurveBinding.PPtrCurve(path, typeof(SpriteRenderer), "m_Sprite");
                AnimationUtility.SetObjectReferenceCurve(animationClip, spriteBinding, spriteKeyframes.ToArray());

                AddEnabledKeyframes(layerTransform, tag, frames, doesLayerDisableRenderer, in activeFrames, in animationClip);
                AddSortOrderKeyframes(layerTransform, layer, tag, frames, in cells, in animationClip);
                AddAnimationEvents(in tag, frames, animationClip);
            }

            return animationClip;
        }

        static HashSet<int> AddCellsToClip(IReadOnlyList<Cell> cells, in Tag tag, in Sprite[] sprites, IReadOnlyList<Frame> frames, ref List<ObjectReferenceKeyframe> keyFrames)
        {
            var activeFrames = new HashSet<int>();
            var startTime = GetTimeFromFrame(frames, tag.fromFrame);
            for (var i = 0; i < cells.Count; ++i)
            {
                var cell = cells[i];
                if (cell.frameIndex < tag.fromFrame ||
                    cell.frameIndex >= tag.toFrame)
                    continue;

                var sprite = Array.Find(sprites, x => x.GetSpriteID() == cell.spriteId);
                if (sprite == null)
                    continue;

                var keyframe = new ObjectReferenceKeyframe();
                var time = GetTimeFromFrame(frames, cell.frameIndex);
                keyframe.time = time - startTime;
                keyframe.value = sprite;
                keyFrames.Add(keyframe);

                activeFrames.Add(cell.frameIndex);
            }
            return activeFrames;
        }

        static HashSet<int> AddLinkedCellsToClip(IReadOnlyList<LinkedCell> linkedCells, in List<Cell> cells, in Tag tag, in Sprite[] sprites, IReadOnlyList<Frame> frames, ref List<ObjectReferenceKeyframe> keyFrames)
        {
            var activeFrames = new HashSet<int>();
            var startTime = GetTimeFromFrame(frames, tag.fromFrame);
            for (var i = 0; i < linkedCells.Count; ++i)
            {
                var linkedCell = linkedCells[i];
                if (linkedCell.frameIndex < tag.fromFrame ||
                    linkedCell.frameIndex >= tag.toFrame)
                    continue;

                var cellIndex = cells.FindIndex(x => x.frameIndex == linkedCell.linkedToFrame);
                if (cellIndex == -1)
                    continue;

                var cell = cells[cellIndex];
                var sprite = Array.Find(sprites, x => x.GetSpriteID() == cell.spriteId);
                if (sprite == null)
                    continue;

                var keyframe = new ObjectReferenceKeyframe();
                var time = GetTimeFromFrame(frames, linkedCell.frameIndex);
                keyframe.time = time - startTime;
                keyframe.value = sprite;
                keyFrames.Add(keyframe);

                activeFrames.Add(linkedCell.frameIndex);
            }
            return activeFrames;
        }

        static void DuplicateLastFrame(ref List<ObjectReferenceKeyframe> keyFrames, Frame lastFrame, float frameRate)
        {
            if (keyFrames.Count == 0)
                return;

            var frameTime = 1f / frameRate;

            var lastKeyFrame = keyFrames[^1];
            var duplicatedFrame = new ObjectReferenceKeyframe();

            var time = lastKeyFrame.time + MsToSeconds(lastFrame.duration);
            // We remove one AnimationClip frame, since the animation system will automatically add one frame at the end.
            time -= frameTime;
            duplicatedFrame.time = time;
            duplicatedFrame.value = lastKeyFrame.value;
            keyFrames.Add(duplicatedFrame);
        }

        static string GetTransformPath(Transform transform)
        {
            var path = transform.name;
            if (transform.name == k_RootName)
                return "";
            if (transform.parent.name == k_RootName)
                return path;

            var parentPath = GetTransformPath(transform.parent) + "/";
            path = path.Insert(0, parentPath);
            return path;
        }

        static void AddEnabledKeyframes(Transform layerTransform, Tag tag, IReadOnlyList<Frame> frames, bool doesLayerDisableRenderer, in HashSet<int> activeFrames, in AnimationClip animationClip)
        {
            if (activeFrames.Count == tag.noOfFrames && !doesLayerDisableRenderer)
                return;

            var path = GetTransformPath(layerTransform);
            var enabledBinding = EditorCurveBinding.FloatCurve(path, typeof(SpriteRenderer), "m_Enabled");
            var enabledKeyframes = new List<Keyframe>();

            var disabledPrevFrame = false;
            var startTime = GetTimeFromFrame(frames, tag.fromFrame);
            for (var frameIndex = tag.fromFrame; frameIndex < tag.toFrame; ++frameIndex)
            {
                var time = GetTimeFromFrame(frames, frameIndex);
                time -= startTime;

                if (!activeFrames.Contains(frameIndex) && !disabledPrevFrame)
                {
                    var keyframe = GetBoolKeyFrame(false, time);
                    enabledKeyframes.Add(keyframe);
                    disabledPrevFrame = true;
                }
                else if (activeFrames.Contains(frameIndex) && disabledPrevFrame)
                {
                    var keyframe = GetBoolKeyFrame(true, time);
                    enabledKeyframes.Add(keyframe);
                    disabledPrevFrame = false;
                }
            }

            if (enabledKeyframes.Count == 0 && !doesLayerDisableRenderer)
                return;

            // Make sure there is an enable keyframe on the first frame, if the first frame is active.
            if (activeFrames.Contains(tag.fromFrame))
            {
                var keyframe = GetBoolKeyFrame(true, 0f);
                enabledKeyframes.Add(keyframe);
            }

            var animCurve = new AnimationCurve(enabledKeyframes.ToArray());
            AnimationUtility.SetEditorCurve(animationClip, enabledBinding, animCurve);
        }

        static void AddSortOrderKeyframes(Transform layerTransform, Layer layer, Tag tag, IReadOnlyList<Frame> frames, in List<Cell> cells, in AnimationClip animationClip)
        {
            var layerGo = layerTransform.gameObject;
            var spriteRenderer = layerGo.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                return;

            var sortOrderKeyframes = new List<Keyframe>();
            var path = GetTransformPath(layerTransform);
            var sortOrderBinding = EditorCurveBinding.FloatCurve(path, typeof(SpriteRenderer), "m_SortingOrder");

            var startTime = GetTimeFromFrame(frames, tag.fromFrame);
            var hasKeyOnFirstFrame = false;
            for (var i = 0; i < cells.Count; ++i)
            {
                var cell = cells[i];
                if (cell.frameIndex < tag.fromFrame ||
                    cell.frameIndex >= tag.toFrame)
                    continue;

                var additiveSortOrder = cell.additiveSortOrder;
                if (additiveSortOrder == 0)
                    continue;

                if (cell.frameIndex == tag.fromFrame)
                    hasKeyOnFirstFrame = true;

                var time = GetTimeFromFrame(frames, cell.frameIndex) - startTime;
                var keyframe = GetIntKeyFrame(layer.index + additiveSortOrder, time);
                sortOrderKeyframes.Add(keyframe);
            }

            if (sortOrderKeyframes.Count == 0)
                return;

            if (!hasKeyOnFirstFrame)
            {
                var firstFrame = GetIntKeyFrame(layer.index, 0f);
                sortOrderKeyframes.Add(firstFrame);
            }

            var animCurve = new AnimationCurve(sortOrderKeyframes.ToArray());
            AnimationUtility.SetEditorCurve(animationClip, sortOrderBinding, animCurve);
        }

        static float GetTimeFromFrame(IReadOnlyList<Frame> frames, int frameIndex)
        {
            var totalMs = 0;
            for (var i = 0; i < frameIndex; ++i)
                totalMs += frames[i].duration;
            return MsToSeconds(totalMs);
        }

        static float MsToSeconds(int ms) => ms / 1000f;

        static Keyframe GetBoolKeyFrame(bool value, float time)
        {
            var keyframe = new Keyframe();
            keyframe.value = value ? 1f : 0f;
            keyframe.time = time;
            keyframe.inTangent = float.PositiveInfinity;
            keyframe.outTangent = float.PositiveInfinity;
            return keyframe;
        }

        static Keyframe GetIntKeyFrame(int value, float time)
        {
            var keyframe = new Keyframe();
            keyframe.value = value;
            keyframe.time = time;
            keyframe.inTangent = float.PositiveInfinity;
            keyframe.outTangent = float.PositiveInfinity;
            return keyframe;
        }

        static void AddAnimationEvents(in Tag tag, IReadOnlyList<Frame> frames, AnimationClip animationClip)
        {
            var events = new List<AnimationEvent>();

            var startTime = GetTimeFromFrame(frames, tag.fromFrame);
            for (var frameIndex = tag.fromFrame; frameIndex < tag.toFrame; ++frameIndex)
            {
                var frame = frames[frameIndex];
                if (frame.eventStrings.Length == 0)
                    continue;

                var frameTime = GetTimeFromFrame(frames, frameIndex);
                var eventStrings = frame.eventStrings;
                for (var m = 0; m < eventStrings.Length; ++m)
                {
                    events.Add(new AnimationEvent()
                    {
                        time = frameTime - startTime,
                        functionName = eventStrings[m]
                    });
                }
            }

            if (events.Count > 0)
                AnimationUtility.SetAnimationEvents(animationClip, events.ToArray());
        }
    }
}
