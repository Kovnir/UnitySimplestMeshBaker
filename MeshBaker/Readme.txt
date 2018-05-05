MeshBaker is a simple plugin which help you to bake several meshesh into one.

Also it contains Bone Baker. It is a simple wrapper over SkinnedMeshRenderer.BakeMesh() method which let you transform SkinnedMeshRendered to MeshRenderer considering transformations and poses in two clicks.



How to use:


To install it copy MeshBaker folder into your project.


To Bake Meshes select GameObjects with MeshRenderer or SkinnedMeshRenderer components on it or on his childs, make a right click and press Bake Meshes. 
One mesh can't contains more than 65000 vertexes, so if models containt. If source meshes together has more than 65000 vertexes Mesh Baker will create several objects.
The plugin will ask you "Do you want to separate objects with different materials?". If you will press "Yes" MeshBaker will bake to the separated Meshes source objects with different materials. If "No" Mesh Baker will bake all meshes together and set him one of the source material.
Also if several but not all meshed uses UVs, Vertex Colors, or Normals Mesh Baker will ask how to resolve it. You can create remove it from all objects, all create with default values where it needed.
Than Mesh Baker will ask you "Do you want to remove sources?". Press "Yes" if so, or "No" if you want to leave this objects for now.
Than Mesh Baker will show popup with count of baked objects.



To Bake Bones select GameObjects with SkinnedMeshRenderer components on it or on his childs, make a right click and press Bake Bones.
The plugin will ask you "Do you want to remove bones after backing?". Press "Yes" if so, or "No" if you want to leave bones for now.



YouTube Tutorials:

How to Bake Meshes - https://youtu.be/wtwlOkyxAw4
How to Bake Bones - https://youtu.be/D6V7tXY23oc



You can report a bug or create a pull request in the GitHub:
https://github.com/Kovnir/BoneRandomizerAndBaker


If you have any problems or ideas, please, contact me:
kovnir.alik@gmail.com


Thanks for using! =)