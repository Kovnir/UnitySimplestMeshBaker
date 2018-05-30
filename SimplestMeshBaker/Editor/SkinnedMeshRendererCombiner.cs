using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SimplestMeshBaker
{
    //Code based on this topic:
    //https://answers.unity.com/questions/625243/combining-skinned-meshes-1.html
    public static class SkinnedMeshRendererCombiner
    {
        [MenuItem("GameObject/Combine SkinnedMeshRenderers", false, 0)]
        private static void Bake(MenuCommand menuCommand)
        {
            if (Selection.gameObjects.Length != 1)
            {
                EditorUtility.DisplayDialog("Simplest Mesh Baker", "You should select one game object.", "Ok");
                return;
            }

            GameObject go = Selection.gameObjects[0];
            if (go.GetComponent<SkinnedMeshRenderer>() != null)
            {
                EditorUtility.DisplayDialog("Simplest Mesh Baker",
                    "You should select game object with SkinnedMeshRenderer on the children, not on the current game object.",
                    "Ok");
                return;
            }

            Vector3 startPosition = go.transform.position;
            Vector3 startScale = go.transform.localScale;
            Quaternion startRotation = go.transform.rotation;

            go.transform.position = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;

            SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            Dictionary<string, List<SkinnedMeshRenderer>> skinnedMeshRenderer = SeparateRenderers(skinnedMeshRenderers);

            foreach (var renderers in skinnedMeshRenderer)
            {
                Process(renderers.Value, go);
            }

            go.transform.position = startPosition;
            go.transform.localScale = startScale;
            go.transform.rotation = startRotation;
        }

        private static Dictionary<string, List<SkinnedMeshRenderer>> SeparateRenderers(
            SkinnedMeshRenderer[] skinnedMeshRenderers)
        {
            Dictionary<string, List<SkinnedMeshRenderer>> result = new Dictionary<string, List<SkinnedMeshRenderer>>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (!result.ContainsKey(skinnedMeshRenderer.sharedMaterial.name))
                {
                    result.Add(skinnedMeshRenderer.sharedMaterial.name, new List<SkinnedMeshRenderer>());
                }
                result[skinnedMeshRenderer.sharedMaterial.name].Add(skinnedMeshRenderer);
            }
            return result;
        }

        private static void Process(List<SkinnedMeshRenderer> skinnedMeshRenderers, GameObject root)
        {
            List<SkinnedMeshRenderer> toDestroy = new List<SkinnedMeshRenderer>();

            List<Transform> bones = new List<Transform>();
            Hashtable bonesByHash = new Hashtable();
            List<BoneWeight> boneWeights = new List<BoneWeight>();

            List<CombineInstance> combineInstances = new List<CombineInstance>();
            int boneIndex = 0;
//            List<Matrix4x4> bindposes = new List<Matrix4x4>();

            Material material = skinnedMeshRenderers[0].sharedMaterial;
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                skinnedMeshRenderer.transform.rotation = Quaternion.identity;
                foreach (Transform bone in skinnedMeshRenderer.bones)
                {
                    bones.Add(bone);
//                    bindposes.Add(bone.worldToLocalMatrix * root.transform.worldToLocalMatrix);
                    if (!bonesByHash.Contains(bone.name))
                    {
                        bonesByHash.Add(bone.name, boneIndex);
                    }
                    boneIndex++;
                }
                FillData(skinnedMeshRenderer, bonesByHash, boneWeights, combineInstances);
                toDestroy.Add(skinnedMeshRenderer);
            }

            SkinnedMeshRenderer newRenderer = Undo.AddComponent<SkinnedMeshRenderer>(root);
            if (newRenderer == null)
            {
                var newObject = new GameObject();
                Undo.RegisterCreatedObjectUndo(newObject, "New Skinned Mesh Renderer object");
                newObject.transform.SetParent(root.transform);
                newRenderer = newObject.AddComponent<SkinnedMeshRenderer>();
            }
            newRenderer.sharedMesh = new Mesh();
            newRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray());

            newRenderer.bones = bones.ToArray();
            newRenderer.sharedMesh.boneWeights = boneWeights.ToArray();
//            newRenderer.sharedMesh.bindposes = bindposes.ToArray();
            newRenderer.sharedMesh.RecalculateBounds();
            newRenderer.sharedMaterial = material;

            foreach (SkinnedMeshRenderer renderer in toDestroy)
            {
                Undo.DestroyObjectImmediate(renderer.gameObject);
            }
        }

        private static void FillData(SkinnedMeshRenderer skinnedMeshRenderer, Hashtable boneHash,
            List<BoneWeight> boneWeights, List<CombineInstance> combineInstances)
        {
            BoneWeight[] meshBoneWeight = skinnedMeshRenderer.sharedMesh.boneWeights;

            foreach (BoneWeight weight in meshBoneWeight)
            {
                BoneWeight boneWeight = weight;

                boneWeight.boneIndex0 = (int) boneHash[skinnedMeshRenderer.bones[weight.boneIndex0].name];
                boneWeight.boneIndex1 = (int) boneHash[skinnedMeshRenderer.bones[weight.boneIndex1].name];
                boneWeight.boneIndex2 = (int) boneHash[skinnedMeshRenderer.bones[weight.boneIndex2].name];
                boneWeight.boneIndex3 = (int) boneHash[skinnedMeshRenderer.bones[weight.boneIndex3].name];

                boneWeights.Add(boneWeight);
            }

            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = skinnedMeshRenderer.sharedMesh;

            combineInstance.transform = skinnedMeshRenderer.transform.localToWorldMatrix;
            combineInstances.Add(combineInstance);
        }
    }
}