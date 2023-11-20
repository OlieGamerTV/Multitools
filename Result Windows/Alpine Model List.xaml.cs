using Alpine.Materials;
using Alpine.Renderables.Models.Skeleton;
using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Utilities.Flash;

namespace Multi_Tool.Result_Windows
{
    /// <summary>
    /// Interaction logic for Alpine_Model_List.xaml
    /// </summary>
    public partial class Alpine_Model_List : Window
    {
        ModelData mesh;
        static readonly XNamespace xnl = "http://www.collada.org/2005/11/COLLADASchema";
        string exportPath = string.Empty;
        public Alpine_Model_List(ModelData data = null)
        {
            InitializeComponent();
            if(data != null )
            {
                mesh = data;
                BuildList();
            }
            this.Show();
        }

        public void BuildList()
        {
            TreeViewItem root = new TreeViewItem();
            root.Header = mesh.name;
            if(mesh.meshes != null)
            {
                TreeViewItem treeMesh = new TreeViewItem();
                treeMesh.Header = "Meshes";
                foreach(var meshes in mesh.meshes)
                {
                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = meshes.name;
                    ListViewItem item = new ListViewItem();
                    item.Content = "ID: " + meshes.id;
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Renderable? " + (meshes.renderable == true ? "Yes" : "No");
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Backface Culling? " + (meshes.backfaceCulling == true ? "Yes" : "No");
                    treeItem.Items.Add(item);
                    treeMesh.Items.Add(treeItem);
                }
                root.Items.Add(treeMesh);
            }
            if(mesh.materials != null)
            {
                TreeViewItem treeMaterial = new TreeViewItem();
                treeMaterial.Header = "Materials";
                for (int i = 0; i < mesh.materials.Count; i++)
                {
                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = mesh.materials[i].Id.ToString();
                    ListViewItem item = new ListViewItem();
                    item.Content = "Constant Inputs [" + string.Join(", ", mesh.materials[i].GetConstantInputNames()) + "]";
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Vertex Attribute Inputs [" + string.Join(", ", mesh.materials[i].GetVertexAttributeInputNames()) + "]";
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Texture Inputs [" + string.Join(", ", mesh.materials[i].GetTextureInputNames()) + "]";
                    treeItem.Items.Add(item);
                    treeMaterial.Items.Add(treeItem);
                }
                root.Items.Add(treeMaterial);
            }
            if (mesh.textures != null)
            {
                TreeViewItem treeTextures = new TreeViewItem();
                treeTextures.Header = "Textures";
                for (int i = 0; i < mesh.textures.Count; i++)
                {
                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = "[" + mesh.textures[i].Name + "] ID - " + mesh.textures[i].Id;
                    ListViewItem item = new ListViewItem();
                    item.Content = "Width - <" + mesh.textures[i].Width.ToString() + ">";
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Height - <" + mesh.textures[i].Height.ToString() + ">";
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Is Downsampling to 16BPP? - " + (mesh.textures[i].downsample16BPP == true ? "Yes" : "No");
                    treeItem.Items.Add(item);
                    item = new ListViewItem();
                    item.Content = "Is Modified? - " + (mesh.textures[i].IsModified == true ? "Yes" : "No");
                    treeItem.Items.Add(item);
                    treeTextures.Items.Add(treeItem);
                }
                root.Items.Add(treeTextures);
            }
            ModelTreeList.Items.Add(root);
        }

        public void WriteToDAE()
        {
            Mouse.OverrideCursor = Cursors.AppStarting;
            XDocument xDocument = new XDocument();
            XDeclaration declaration = new XDeclaration("1.0", "utf-8", null);
            xDocument.Declaration = declaration;
            XElement collada = new XElement(xnl + "COLLADA");
            XAttribute schema = new XAttribute(XNamespace.Xmlns.GetName("xsi"), "http://www.w3.org/2001/XMLSchema-instance");
            XAttribute ver = new XAttribute("version", "1.4.1");
            collada.Add(schema);
            collada.Add(ver);
            xDocument.Add(collada);
            xDocument.Root.Add(WriteDAEAsset());
            xDocument.Root.Add(WriteDAEEffectLibrary());
            xDocument.Root.Add(WriteDAEImageLibrary());
            xDocument.Root.Add(WriteDAEMaterialLibrary());
            xDocument.Root.Add(WriteDAEGeoLibrary());
            if (mesh.hasSkeletonData)
            {
                xDocument.Root.Add(WriteDAEControllerLibrary());
            }
            xDocument.Root.Add(WriteDAESceneLibrary());
            XElement scene = new XElement("scene");
            XElement instVisScenes = new XElement("instance_visual_scene", new XAttribute("url", "#Scene"));
            scene.Add(instVisScenes);
            xDocument.Root.Add(scene);
            Mouse.OverrideCursor = null;
            xDocument.Save(exportPath);
            Debug.WriteLine("Exported to " + exportPath);
            foreach (var texture in mesh.textures)
            {
                BinaryWriter writer = new BinaryWriter(File.Create(exportPath.Remove(exportPath.LastIndexOf('\\') + 1) + "\\" + texture.Name + texture.Extension));
                writer.Write(texture.RawData.data.ToArray());
                writer.Flush();
                writer.Close();
                Debug.WriteLine(texture.Name + texture.Extension + " Exported to " + exportPath.Remove(exportPath.LastIndexOf('\\') + 1));
            }
        }

        #region DAE Library Writing
        private XElement WriteDAEAsset()
        {
            XElement asset = new XElement(xnl + "asset");
            XElement contributor = new XElement(xnl + "contributor");
            XElement author = new XElement(xnl + "author", "Multitools User");
            XElement authoringTool = new XElement(xnl + "authoring_tool", "Multitools Alpine Extractor Tool. Extract Time: " + System.DateTime.Now.ToShortTimeString());
            contributor.Add(author);
            contributor.Add(authoringTool);
            XElement created = new XElement(xnl + "created", DateTime.Now.ToLongDateString());
            contributor.Add(created);
            asset.Add(contributor);
            return asset;
        }

        private XElement WriteDAEGeoLibrary()
        {
            XElement libraryGeo = new XElement(xnl + "library_geometries");
            foreach (var meshes in mesh.meshes)
            {
                XElement geometry = new XElement(xnl + "geometry", new XAttribute("id", meshes.name + "-Mesh"), new XAttribute("name", meshes.name));
                XElement mesh = new XElement(xnl + "mesh");
                mesh.Add(WriteDaeGeoPos(meshes));
                mesh.Add(WriteDaeGeoNorm(meshes));
                if (meshes.vertexData.texCoords.ContainsKey("Color-Unused-uv-0"))
                {
                    mesh.Add(WriteDaeGeoUV(meshes));
                }
                XElement vertices = new XElement(xnl + "vertices", 
                    new XAttribute("id", meshes.name + "-" + meshes.id + "-Verts"));
                XElement vertInput = new XElement(xnl + "input", 
                    new XAttribute("semantic", "POSITION"), 
                    new XAttribute("source", "#" + meshes.name + "-" + meshes.id + "-Pos"));
                vertices.Add(vertInput);
                mesh.Add(vertices);
                mesh.Add(WriteDaeGeoTris(meshes));
                geometry.Add(mesh);
                libraryGeo.Add(geometry);
            }
            return libraryGeo;
        }

        private XElement WriteDAESceneLibrary()
        {
            XText matrix = new XText("1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1");
            XElement libraryScenes = new XElement(xnl + "library_visual_scenes");
            XElement visScene = new XElement(xnl + "visual_scene", 
                new XAttribute("id", "Scene"), 
                new XAttribute("name", "Scene"));
            if (mesh.hasSkeletonData)
            {
                visScene.Add(WriteSceneSkeleton());
            }
            else
            {
                foreach (var meshes in mesh.meshes)
                {
                    XElement node = new XElement(xnl + "node",
                        new XAttribute("id", meshes.name + "-Node"),
                        new XAttribute("name", meshes.name),
                        new XAttribute("type", "NODE"));
                    XElement transformMat = new XElement(xnl + "matrix",
                        new XAttribute("sid", "transform"));
                    transformMat.Add(matrix);
                    XElement instGeo = new XElement(xnl + "instance_geometry",
                        new XAttribute("url", "#" + meshes.name + "-Mesh"),
                        new XAttribute("name", meshes.name));
                    XElement bindMat = new XElement(xnl + "bind_material");
                    XElement techCommon = new XElement(xnl + "technique_common");
                    XElement instMat = new XElement(xnl + "instance_material", new XAttribute("symbol", meshes.name + "-Mat"), new XAttribute("target", "#" + meshes.material.Id));
                    XElement binder = new XElement(xnl + "bind_vertex_input", new XAttribute("semantic", "Color-Unused-uv-0"), new XAttribute("input_semantic", "TEXCOORD"), new XAttribute("input_set", "0"));
                    instMat.Add(binder);
                    techCommon.Add(instMat);
                    bindMat.Add(techCommon);
                    instGeo.Add(bindMat);
                    node.Add(transformMat, instGeo);
                    visScene.Add(node);
                }
            }
            libraryScenes.Add(visScene);
            return libraryScenes;
        }

        private XElement WriteDAEImageLibrary()
        {
            XElement libTexture = new XElement(xnl + "library_images");
            foreach (var texture in mesh.textures)
            {
                XElement image = new XElement(xnl + "image",
                    new XAttribute("id", texture.Name),
                    new XAttribute("name", texture.Name));
                XElement init = new XElement(xnl + "init_from");
                XText path = new XText(texture.Name + texture.Extension);
                init.Add(path);
                image.Add(init);
                libTexture.Add(image);
            }
            return libTexture;
        }

        private XElement WriteDAEMaterialLibrary()
        {
            XElement matLibrary = new XElement(xnl + "library_materials");
            foreach (var meshes in mesh.materials)
            {
                XElement material = new XElement(xnl + "material", new XAttribute("id", meshes.Id));
                XElement instanceEffect = new XElement(xnl + "instance_effect", 
                    new XAttribute("url", "#defaultEffect" + meshes.Id));
                material.Add(instanceEffect);
                matLibrary.Add(material);
            }
            return matLibrary;
        }

        private XElement WriteDAEEffectLibrary()
        {
            XElement libEffects = new XElement(xnl + "library_effects");
            foreach (var meshes in mesh.materials)
            {
                XElement effect = new XElement(xnl + "effect", new XAttribute("id", "defaultEffect" + meshes.Id));
                XElement profile = new XElement(xnl + "profile_COMMON");
                XElement technique = new XElement(xnl + "technique", new XAttribute("sid", "common"));
                XElement phong = new XElement(xnl + "lambert");
                if (meshes.HasConstantInput(ConstantInputName.AMBIENT))
                {
                    XElement color = new XElement(xnl + "color", new XAttribute("sid", "ambient"));
                    XElement ambient = new XElement(xnl + "ambient");
                    XText ambientCol = new XText(string.Join(' ', meshes.GetConstantInput(ConstantInputName.AMBIENT)));
                    color.Add(ambientCol);
                    ambient.Add(color);
                    phong.Add(ambient);
                }
                if (meshes.HasConstantInput(ConstantInputName.DIFFUSE))
                {
                    XElement color = new XElement(xnl + "color", new XAttribute("sid", "diffuse"));
                    XElement diffuse = new XElement(xnl + "diffuse");
                    XText diffuseCol = new XText(string.Join(' ', meshes.GetConstantInput(ConstantInputName.DIFFUSE)));
                    color.Add(diffuseCol);
                    diffuse.Add(color);
                    phong.Add(diffuse);
                }
                if (meshes.HasTextureInput(TextureInputName.DIFFUSE_MAP))
                {
                    XElement text = new XElement(xnl + "texture", new XAttribute("texture", "Color-0_sample"), new XAttribute("texcoord", "Color-Unused-uv-0"));
                    XElement newParam1 = new XElement(xnl + "newparam", new XAttribute("sid", mesh.GetPackedTexture("Color-0").Name + "_surface"));
                    XElement surf = new XElement(xnl + "surface", new XAttribute("type", "2D"));
                    XElement init = new XElement(xnl + "init_from", "Color-0");
                    surf.Add(init); newParam1.Add(surf); profile.Add(newParam1);
                    XElement newParam2 = new XElement(xnl + "newparam", new XAttribute("sid", mesh.GetPackedTexture("Color-0").Name + "_sample"));
                    XElement sampler2D = new XElement(xnl + "sampler2D");
                    XElement source = new XElement(xnl + "source", mesh.GetPackedTexture("Color-0").Name + "_surface");
                    sampler2D.Add(source); newParam2.Add(sampler2D); profile.Add(newParam2);
                    XElement diffuse = new XElement(xnl + "diffuse");
                    diffuse.Add(text); phong.Add(diffuse);
                }
                if (meshes.HasConstantInput(ConstantInputName.TRANSPARENCY))
                {
                    XElement color = new XElement(xnl + "color", new XAttribute("sid", "transparent"));
                    XElement transparent = new XElement(xnl + "transparent");
                    XText transparentCol = new XText(meshes.GetConstantInput(ConstantInputName.TRANSPARENCY)[0].ToString());
                    color.Add(transparentCol);
                    transparent.Add(color);
                    phong.Add(transparent);
                }
                technique.Add(phong);
                profile.Add(technique);
                effect.Add(profile);
                libEffects.Add(effect);
            }
            return libEffects;
        }

        private XElement WriteDAEControllerLibrary()
        {
            XElement libController = new XElement(xnl + "library_controllers");
            if (mesh.skeleton != null)
            {
                foreach (var meshes in mesh.meshes)
                {
                    XElement control = new XElement("controller", new XAttribute("id", meshes.name + "-skin"));
                    XElement skin = new XElement("skin", new XAttribute("source", "#" + meshes.name + "-Mesh"));

                    // Joint Names
                    XElement srcJoints = new XElement("source", new XAttribute("id", meshes.name + "-Joints"));
                    XElement jointsNameArr = new XElement("Name_array", new XAttribute("id", meshes.name + "-Joints-array"), new XAttribute("count", meshes.joints.Count));
                    foreach (var joint in meshes.joints)
                    {
                        jointsNameArr.Add(mesh.skeleton[joint].name + " ");
                    }
                    srcJoints.Add(jointsNameArr);
                    XElement techCommon = new XElement("technique_common");
                    XElement accessor = new XElement("accessor", 
                        new XAttribute("source", "#" + meshes.name + "-Joints-array"), 
                        new XAttribute("count", meshes.joints.Count), 
                        new XAttribute("stride", 1));
                    XElement param = new XElement("param", 
                        new XAttribute("name", "JOINT"), 
                        new XAttribute("type", "name"));
                    accessor.Add(param);
                    techCommon.Add(accessor);
                    srcJoints.Add(techCommon);
                    skin.Add(srcJoints);

                    // Weights
                    XElement srcWeights = new XElement("source", new XAttribute("id", meshes.name + "-Weights"));
                    List<float> weights = FileUtils.ReadNumberVectorToList(meshes.vertexData.weights, 0);
                    XElement floatArr = new XElement("float_array", new XAttribute("id", meshes.name + "-Weights-array"), new XAttribute("count", weights.Count));
                    floatArr.Add(string.Join(" ", weights));
                    srcWeights.Add(floatArr);
                    techCommon = new XElement("technique_common");
                    accessor = new XElement("accessor",
                        new XAttribute("source", "#" + meshes.name + "-Weights-array"),
                        new XAttribute("count", weights.Count),
                        new XAttribute("stride", 1));
                    param = new XElement("param",
                        new XAttribute("name", "WEIGHT"),
                        new XAttribute("type", "float"));
                    accessor.Add(param);
                    techCommon.Add(accessor);
                    srcWeights.Add(techCommon);
                    skin.Add(srcWeights);

                    // Inv Binds
                    XElement srcBinds = new XElement("source", new XAttribute("id", meshes.name + "-Inv_Bind_Mats"));
                    List<float> bindMats = FileUtils.ReadNumberVectorToList(meshes.vertexData.joints, 0);
                    XElement floatBindArr = new XElement("float_array", new XAttribute("id", meshes.name + "-Inv_Bind_Mats-array"), new XAttribute("count", bindMats.Count));
                    floatBindArr.Add(string.Join(" ", bindMats));
                    srcBinds.Add(floatBindArr);
                    techCommon = new XElement("technique_common");
                    accessor = new XElement("accessor",
                        new XAttribute("source", "#" + meshes.name + "-Inv_Bind_Mats-array"),
                        new XAttribute("count", weights.Count / 12),
                        new XAttribute("stride", 12));
                    param = new XElement("param",
                        new XAttribute("name", "TRANSFORM"),
                        new XAttribute("type", "float4x3"));
                    accessor.Add(param);
                    techCommon.Add(accessor);
                    srcBinds.Add(techCommon);
                    skin.Add(srcBinds);

                    // Joints
                    XElement joints = new XElement("joints");
                    XElement inputJoints = new XElement("input", 
                        new XAttribute("semantic", "JOINT"), 
                        new XAttribute("source", "#" + meshes.name + "-Joints"));
                    XElement inputBinds = new XElement("input",
                        new XAttribute("semantic", "INV_BIND_MATRIX"),
                        new XAttribute("source", "#" + meshes.name + "-Inv_Bind_Mats"));
                    joints.Add(inputJoints, inputBinds);
                    skin.Add(joints);

                    // Vertex Weights
                    XElement vertWeight = new XElement("vertex_weights", new XAttribute("count", "0"));
                    XElement inputJoint = new XElement("input", new XAttribute("semantic", "JOINT"), new XAttribute("offset", "0"), new XAttribute("source", "#" + meshes.name + "-Joints"));
                    XElement inputWeights = new XElement("input", new XAttribute("semantic", "WEIGHT"), new XAttribute("offset", "1"), new XAttribute("source", "#" + meshes.name + "-Weights"));
                    vertWeight.Add(inputJoint, inputWeights);
                    skin.Add(vertWeight);
                    control.Add(skin);
                    libController.Add(control);
                }
            }
            return libController;
        }
        #endregion

        #region DAE Library Geometry Writing
        private XElement WriteDaeGeoPos(MeshData meshData)
        {
            Debug.Write("Reading Geo Positions...");
            List<float> floats = new List<float>();
            XElement sourcePos = new XElement(xnl + "source", new XAttribute("id", meshData.name + "-" + meshData.id + "-Pos"));
            meshData.vertexData.vertices.Position = 0;
            List<float> positions = FileUtils.ReadNumberVectorToList(meshData.vertexData.vertices, 0);
            XElement posArray = new XElement(xnl + "float_array", 
                new XAttribute("id", meshData.name + "-" + meshData.id + "-Pos_Array"), 
                new XAttribute("count", positions.Count));
            foreach (var position in positions)
            {
                floats.Add(position);
                if (floats.Count >= 3)
                {
                    XText pos = new XText(string.Join(' ', floats.ToArray()) + " ");
                    posArray.Add(pos);
                    floats = new List<float>();
                }
            }
            sourcePos.Add(posArray);
            XElement technique = new XElement(xnl + "technique_common");
            XElement accessor = new XElement(xnl + "accessor", 
                new XAttribute("source", "#" + meshData.name + "-" + meshData.id + "-Pos_Array"), 
                new XAttribute("count", (positions.Count / 3)), 
                new XAttribute("stride", 3));
            XElement xAxis = new XElement(xnl + "param",
                new XAttribute("name", "X"),
                new XAttribute("type", "float"));
            XElement yAxis = new XElement(xnl + "param",
                new XAttribute("name", "Y"),
                new XAttribute("type", "float"));
            XElement zAxis = new XElement(xnl + "param",
                new XAttribute("name", "Z"),
                new XAttribute("type", "float"));
            accessor.Add(xAxis, yAxis, zAxis);
            technique.Add(accessor);
            sourcePos.Add(technique);
            return sourcePos;
        }

        private XElement WriteDaeGeoNorm(MeshData meshData)
        {
            Debug.Write("Reading Geo Normals...");
            List<float> floats = new List<float>();
            XElement sourceNorm = new XElement(xnl + "source", new XAttribute("id", meshData.name + "-" + meshData.id + "-Normals"));
            meshData.vertexData.normals.Position = 0;
            List<float> normals = FileUtils.ReadNumberVectorToList(meshData.vertexData.normals, 0);
            XElement normArray = new XElement(xnl + "float_array",
                new XAttribute("id", meshData.name + "-" + meshData.id + "-Normals_Array"),
                new XAttribute("count", normals.Count));
            foreach (var position in normals)
            {
                floats.Add(position);
                if (floats.Count >= 3)
                {
                    XText normal = new XText(string.Join(' ', floats.ToArray()) + " ");
                    normArray.Add(normal);
                    floats = new List<float>();
                }
            }
            sourceNorm.Add(normArray);
            XElement technique = new XElement(xnl + "technique_common");
            XElement accessor = new XElement(xnl + "accessor",
                new XAttribute("source", "#" + meshData.name + "-" + meshData.id + "-Normals_Array"),
                new XAttribute("count", (normals.Count / 3)),
                new XAttribute("stride", 3));
            XElement xAxis = new XElement(xnl + "param",
                new XAttribute("name", "X"),
                new XAttribute("type", "float"));
            XElement yAxis = new XElement(xnl + "param",
                new XAttribute("name", "Y"),
                new XAttribute("type", "float"));
            XElement zAxis = new XElement(xnl + "param",
                new XAttribute("name", "Z"),
                new XAttribute("type", "float"));
            accessor.Add(xAxis, yAxis, zAxis);
            technique.Add(accessor);
            sourceNorm.Add(technique);
            return sourceNorm;
        }

        private XElement WriteDaeGeoUV(MeshData meshData)
        {
            Debug.Write("Reading Geo UV Coordinates...");
            List<float> floats = new List<float>();
            XElement sourceUV = new XElement(xnl + "source", new XAttribute("id", "Color-Unused-uv-0"));
            meshData.vertexData.texCoords["Color-Unused-uv-0"].data.Position = 0;
            List<float> uvCoords = FileUtils.ReadNumberVectorToList(meshData.vertexData.texCoords["Color-Unused-uv-0"].data, 0, 2);
            for (int i = 1; i < uvCoords.Count; i += 2)
            {
                uvCoords[i] = -uvCoords[i] + 1;
            }
            XElement posArray = new XElement(xnl + "float_array",
                new XAttribute("id", "Color-Unused-uv-0" + "-Mesh-Map_Array"),
                new XAttribute("count", uvCoords.Count));
            foreach (var coord in uvCoords)
            {
                floats.Add(coord);
                if (floats.Count >= 2)
                {
                    XText pos = new XText(string.Join(' ', floats.ToArray()) + " ");
                    posArray.Add(pos);
                    floats = new List<float>();
                }
            }
            sourceUV.Add(posArray);
            XElement technique = new XElement(xnl + "technique_common");
            XElement accessor = new XElement(xnl + "accessor",
                new XAttribute("source", "#Color-Unused-uv-0" + "-Mesh-Map_Array"),
                new XAttribute("count", (uvCoords.Count / 2)),
                new XAttribute("stride", 2));
            XElement xAxis = new XElement(xnl + "param",
                new XAttribute("name", "S"),
                new XAttribute("type", "float"));
            XElement yAxis = new XElement(xnl + "param",
                new XAttribute("name", "T"),
                new XAttribute("type", "float"));
            accessor.Add(xAxis, yAxis);
            technique.Add(accessor);
            sourceUV.Add(technique);
            return sourceUV;
        }

        private XElement WriteDaeGeoTris(MeshData meshData)
        {
            Debug.Write("Reading Mesh [" + meshData.name + "] indices...");
            List<short> indicesList = ReadIndicesToList(meshData.indices, meshData.indicesAmount);
            XElement tris = new XElement(xnl + "polylist", 
                new XAttribute("count", indicesList.Count / 3));
            XElement inputVertex = new XElement(xnl + "input", 
                new XAttribute("semantic", "VERTEX"), 
                new XAttribute("source", "#" + meshData.name + "-" + meshData.id + "-Verts"), 
                new XAttribute("offset", 0));
            XElement inputNormal = new XElement(xnl + "input",
                new XAttribute("semantic", "NORMAL"),
                new XAttribute("source", "#" + meshData.name + "-" + meshData.id + "-Normals"),
                new XAttribute("offset", 0));
            if (meshData.vertexData.texCoords.ContainsKey("Color-Unused-uv-0"))
            {
                XElement inputTexCoord = new XElement(xnl + "input",
                new XAttribute("semantic", "TEXCOORD"),
                new XAttribute("source", "#Color-Unused-uv-0"),
                new XAttribute("offset", 0),
                new XAttribute("set", 0));
                tris.Add(inputVertex, inputNormal, inputTexCoord);
            }
            else
            {
                tris.Add(inputVertex, inputNormal);
            }
            XElement vCount = new XElement("vcount");
            for (int i = 0; i < indicesList.Count / 3; i++)
            {
                vCount.Add(3 + " ");
            }
            tris.Add(vCount);
            XElement triIndices = new XElement(xnl + "p", string.Join("  ", indicesList.ToArray()));
            tris.Add(triIndices);
            return tris;
        }
        #endregion

        private XElement WriteSceneSkeleton()
        {
            XElement rootNode = new XElement("node",
                    new XAttribute("id", "skeleton"),
                    new XAttribute("name", "Armature Rig"),
                    new XAttribute("sid", "skeleton"),
                    new XAttribute("type", "NODE"));
            XElement translateRoot = new XElement("translate", new XText("0 0 0"));
            XElement scaleRoot = new XElement("scale", new XText("1 1 1"));
            rootNode.Add(translateRoot, scaleRoot);
            Dictionary<int, XElement> joints = new Dictionary<int, XElement>();
            for (int i = 0; i < mesh.skeleton.Count; i++)
            {
                XElement joint = new XElement("node", 
                    new XAttribute("id", mesh.skeleton[i].name), 
                    new XAttribute("name", mesh.skeleton[i].name),
                    new XAttribute("sid", mesh.skeleton[i].name),
                    new XAttribute("type", "JOINT"));
                XElement translate = new XElement("translate", new XText(mesh.skeleton[i].transform.translation.XYZToString()));
                XElement scale = new XElement("scale", new XText(string.Join(" ", mesh.skeleton[i].transform.scale, mesh.skeleton[i].transform.scale, mesh.skeleton[i].transform.scale)));
                joint.Add(translate, scale);
                joints.Add(mesh.skeleton[i].id, joint);
            }
            for (int i = joints.Count - 1; i >= 0; i--)
            {
                if (joints.ContainsKey((int)mesh.skeleton[i].parentId))
                {
                    joints[(int)mesh.skeleton[i].parentId].Add(joints[i]);
                    joints.Remove(i);
                }
            }
            for (int i = 0; i < joints.Count; i++)
            {
                rootNode.Add(joints[i]);
            }
            rootNode.Add(WriteSceneSkinnedMesh());
            return rootNode;
        }

        private XElement[] WriteSceneSkinnedMesh()
        {
            List<XElement> nodes = new List<XElement>();
            for (int i = 0; i < mesh.meshes.Count; i++)
            {
                XElement node = new XElement(xnl + "node", new XAttribute("id", mesh.meshes[i].name), new XAttribute("type", "NODE"));
                XElement instController = new XElement(xnl + "instance_controller", new XAttribute("url", "#" + mesh.meshes[i].name + "-skin"));
                for (int joint = 0; joint < mesh.meshes[i].joints.Count; joint++)
                {
                    XElement skeleton = new XElement(xnl + "skeleton", new XText("#" + mesh.skeleton[mesh.meshes[i].joints[joint]].name));
                    instController.Add(skeleton);
                }
                XElement bindMat = new XElement(xnl + "bind_material");
                XElement techCommon = new XElement(xnl + "technique_common");
                XElement instMat = new XElement(xnl + "instance_material", new XAttribute("symbol", mesh.meshes[i].name + "-Mat"), new XAttribute("target", "#" + mesh.meshes[i].material.Id));
                XElement binder = new XElement(xnl + "bind_vertex_input", new XAttribute("semantic", "Color-Unused-uv-0"), new XAttribute("input_semantic", "TEXCOORD"), new XAttribute("input_set", "0"));
                instMat.Add(binder);
                techCommon.Add(instMat);
                bindMat.Add(techCommon);
                instController.Add(bindMat);
                node.Add(instController);
                nodes.Add(node);
            }
            return nodes.ToArray();
        }

        public void WriteToOBJ()
        {
            StreamWriter writer = new StreamWriter(File.Create(exportPath));
            
            List<float> positions = new List<float>();
            List<float> normals = new List<float>();
            List<short> indices = new List<short>();
            foreach (var meshes in mesh.meshes)
            {
                writer.Write(meshes.name + "\n");
                meshes.vertexData.vertices.Position = 0;
                positions = FileUtils.ReadNumberVectorToList(meshes.vertexData.vertices, 0);
                WriteOBJPositions(writer, positions);
                meshes.vertexData.normals.Position = 0;
                normals = FileUtils.ReadNumberVectorToList(meshes.vertexData.normals, 0);
                WriteOBJNormals(writer, normals);
                if (meshes.vertexData.texCoords.ContainsKey("Color-Unused-uv-0"))
                {
                    meshes.vertexData.texCoords["Color-Unused-uv-0"].data.Position = 0;
                    List<float> texCoord = FileUtils.ReadNumberVectorToList(meshes.vertexData.texCoords["Color-Unused-uv-0"].data, 0, 2);
                    WriteOBJTexCoords(writer, texCoord);
                }
            }
            foreach(var meshes in mesh.meshes)
            {
                writer.Write("# g " + meshes.name + "\n");
                indices = ReadIndicesToList(meshes.indices, meshes.indicesAmount);
                WriteOBJIndices(writer, indices);
            }
            writer.Flush();
            writer.Close();
        }

        #region OBJ Data Writing
        public void WriteOBJPositions(StreamWriter writer, List<float> positionsList)
        {
            int count = 0;
            int amountRead = 0;
            writer.Write("v ");
            foreach (float position in positionsList)
            {
                count++;
                amountRead++;
                if (count != 3)
                {
                    writer.Write(position + " ");
                }
                if (count >= 3)
                {
                    writer.Write(position);
                }
                if (count >= 3 && amountRead != positionsList.Count)
                {
                    writer.Write("\nv ");
                    count = 0;
                }
                else if (count >= 3 && amountRead >= positionsList.Count)
                {
                    writer.Write("\n");
                    count = 0;
                }
            }
        }

        public void WriteOBJNormals(StreamWriter writer, List<float> normalsList)
        {
            int count = 0;
            int amountRead = 0;
            writer.Write("vn ");
            foreach (float normal in normalsList)
            {
                count++;
                amountRead++;
                if (count != 3)
                {
                    writer.Write(normal + " ");
                }
                if (count >= 3)
                {
                    writer.Write(normal);
                }
                if (count >= 3 && amountRead != normalsList.Count)
                {
                    writer.Write("\nvn ");
                    count = 0;
                }
                else if (count >= 3 && amountRead >= normalsList.Count)
                {
                    writer.Write("\n");
                    count = 0;
                }
            }
        }

        public void WriteOBJTexCoords(StreamWriter writer, List<float> texCoordsList)
        {
            int count = 0;
            int amountRead = 0;
            writer.Write("vt ");
            foreach (float coord in texCoordsList)
            {
                count++;
                amountRead++;
                if (count != 2)
                {
                    writer.Write(coord + " ");
                }
                if (count >= 2)
                {
                    writer.Write(coord);
                }
                if (count >= 2 && amountRead != texCoordsList.Count)
                {
                    writer.Write("\nvt ");
                    count = 0;
                }
                else if (count >= 2 && amountRead >= texCoordsList.Count)
                {
                    writer.Write("\n");
                    count = 0;
                }
            }
        }

        public void WriteOBJIndices(StreamWriter writer, List<short> indicesList)
        {
            int count = 0;
            int amountRead = 0;
            writer.Write("f ");
            foreach (short coord in indicesList)
            {
                count++;
                amountRead++;
                if (count != 3)
                {
                    writer.Write(coord + "// ");
                }
                if (count >= 3)
                {
                    writer.Write(coord + "//");
                }
                if (count >= 3 && amountRead != indicesList.Count)
                {
                    writer.Write("\nf ");
                    count = 0;
                }
                else if (count >= 3 && amountRead >= indicesList.Count)
                {
                    writer.Write("\n");
                    count = 0;
                }
            }
        }
        #endregion

        private List<short> ReadIndicesToList(ByteArray param1, int length)
        {
            Debug.WriteLine("Reading Indice.");
            param1.Position = 0;
            if (param1 == null)
            {
                Debug.WriteLine("Failed to read Indices.");
                return null;
            }
            int loc8 = 0;
            List<short> uints = new List<short>();
            try
            {
                while (loc8 < length)
                {
                    uints.Add(param1.ReadShort());
                    loc8++;
                }
                Debug.WriteLine("Read Indices: <" + string.Join(", ", uints) + ">");
                return uints;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while reading indices, skipping rest and moving to next step.");
                Debug.WriteLine("Read Indices: <" + string.Join(", ", uints) + ">");
                return uints;
            }
        }

        private void SaveDialog()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save Model Data";
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "Collada DAE (*.dae)|*.dae|Wavefront OBJ [W.I.P] (*.obj)|*.obj";
            fileDialog.ValidateNames = true;
            if (fileDialog.ShowDialog() == true)
            {
                exportPath = fileDialog.FileName;
                return;
            }
            else
            {
                exportPath = "";
                return;
            }
        }

        private void ExportDae_Click(object sender, RoutedEventArgs e)
        {
            SaveDialog();
            switch (System.IO.Path.GetExtension(exportPath))
            {
                case ".obj":
                    WriteToOBJ();
                    return;
                case ".dae":
                    WriteToDAE();
                    return;
                default:
                    return;
            }
        }
    }
}
