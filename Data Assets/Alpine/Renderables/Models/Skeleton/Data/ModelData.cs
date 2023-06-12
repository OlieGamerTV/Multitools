using Alpine.Geom;
using Alpine.Materials;
using Alpine.Textures;
using Alpine.Util;
using Multi_Tool.Tools;
using Multi_Tool.Tools.BKV;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DrawImage = System.Drawing.Image;
using System.Windows.Controls;
using Multi_Tool.Utilities;
using System.Reflection.Metadata;
using System.IO;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class ModelData
    {
        public static int VERSION = 10;
        public static List<int> SUPPORTED_VERSIONS = new List<int>{ 9, 10 };

        public ByteArray? texturesFile;
        public string? name;
        public List<PackedTexture>? textures;
        public List<VertexData>? vertexDatas;
        public List<MeshData>? meshes;
        public List<BlendShapeData>? blendShapes;
        public List<ByteArray>? indices;
        public List<JointData>? skeleton;
        public List<Alpine.Materials.Material>? materials;
        public List<LodData>? lods;
        public List<IBoundingVolume> boundingVolumes;
        public List<PointData>? points;
        public List<CurveData>? curves;
        private int tempIndiceAmount;
        private int refs;
        private int loadCount;
        private bool downsample16BPP;
        private OutputLog outputter;

        public ModelData(ListView list = null, string ? param1 = null, ByteArray? param2 = null) : base()
        {
            outputter = new OutputLog(list);
        }

        #region General File Events
        public void Destroy()
        {
            int loc1 = 0;
            int loc2 = 0;
            int loc3 = 0;
            if (refs > 0)
            {
                Debug.Fail("ModelData [" + this.name + "] is still referenced (" + this.refs + ")! Destroying...");
            }
            while (refs > 0)
            {

            }
            if (meshes != null)
            {
                loc3 = meshes.Count();
                loc1 = 0;
                while (loc1 < loc3)
                {
                    this.meshes[loc1].Destroy();
                    loc1++;
                }
                meshes = null;
            }
            if (vertexDatas != null)
            {
                loc3 = vertexDatas.Count();
                loc1 = 0;
                while (loc1 < loc3)
                {
                    vertexDatas[loc1].Destroy();
                    loc1++;
                }
                vertexDatas = null;
            }
            if (textures != null)
            {
                loc3 = textures.Count();
                loc1 = 0;
                while (loc1 < loc3)
                {
                    textures[loc1].Destroy();
                    loc1++;
                }
                textures = null;
            }
            if (materials != null)
            {
                loc3 = materials.Count();
                loc1 = 0;
                while (loc1 < loc3)
                {
                    materials[loc1].Destroy();
                    loc1++;
                }
                materials = null;
            }
            blendShapes = null;
            indices = null;
            skeleton = null;
        }

        public void Build(string param1, dynamic param2, dynamic param3, dynamic param4, dynamic param5, dynamic param6, Material param7)
        {
            name = param1;
            textures = new List<PackedTexture>();
            vertexDatas = new List<VertexData>();
            meshes = new List<MeshData>();
            indices = new List<ByteArray>();
            materials = new List<Alpine.Materials.Material>();
            if (!(param2 is ByteArray))
            {
                param2 = AlpineUtils.CreateByteArray(param2);
            }
            if (param3 != null && !(param3 is ByteArray))
            {
                param3 = AlpineUtils.CreateByteArray(param3);
            }
            if (param4 != null && !(param4 is ByteArray))
            {
                param4 = AlpineUtils.CreateByteArray(param4);
            }
            if (param5 != null && !(param5 is ByteArray))
            {
                param5 = AlpineUtils.CreateByteArray(param5);
            }
            if (!(param6 is ByteArray))
            {
                param6 = AlpineUtils.CreateByteArray(param6);
            }
            VertexData loc8;
            (loc8 = new VertexData()).Build(param2, param3, param4, param5);
            MeshData loc9;
            (loc9 = new MeshData()).Build(param6);
            loc9.vertexDataId = AddVertexData(loc8);
            loc9.materialId = AddMaterial(param7);
            AddMesh(loc9);
        }

        public bool Load(string name, ByteArray mainFile, ByteArray textureFile)
        {
            outputter.WriteToOutput("ModelData.load: Loading data...");
            Dictionary<string, ByteArray> loc5 = null;
            if (name == null)
            {
                throw new ArgumentException("Invalid name: " + name);
            }
            this.name = name;
            Debug.WriteLine("ModelData.Load: Name: " + name);
            DPack dPack = new DPack(outputter.GetLog);
            outputter.WriteToOutput("ModelData.Load: Initializing DPack Reader...", "-----------------------------------");
            Dictionary<string, ByteArray> loc4;
            if ((loc4 = dPack.Unpack(mainFile, false)) == null)
            {
                Debug.WriteLine("File is not a DPack.");
                return false;
            }
            outputter.WriteToOutput("-----------------------------------", "ModelData.Load: Main File Loaded.");
            if (textureFile != null)
            {
                if ((loc5 = dPack.Unpack(textureFile, false)) == null)
                {
                    throw new ArgumentException("Textures file is not a DPack");
                }
                Debug.WriteLine("ModelData.Load: Textures file unpacked.");
                foreach (string loc6 in loc5.Keys)
                {
                    loc4.Add(loc6, loc5[loc6]);
                }
                outputter.WriteToOutput("-----------------------------------", "ModelData.Load: Textures File Loaded.");
            }
            BKVReader reader = new BKVReader(outputter.GetLog);
            if (reader.IsBKV(loc4["desc"]))
            {
                return this.LoadBKV(loc4);
            }
            return false;
        }

        private bool LoadBKV(Dictionary<string, ByteArray> param1)
        {
            BKVTable loc6 = null, loc9 = null, loc10 = null;
            int loc7 = 0, loc8 = 0;
            string loc11 = null;
            MeshData loc12 = null;
            BKVReader loc2 = new BKVReader(outputter.GetLog);
            outputter.WriteToOutput("Loading BKV for ModelData [" + name + "]...");
            loc2.Load(param1["desc"]);
            Console.WriteLine("BKV for ModelData [" + name + "] loaded.");
            outputter.WriteToOutput("-----------------------------------", "BKV for ModelData [" + name + "] loaded.");
            BKVTable loc3 = loc2.GetRoot();
            int loc4 = loc3.GetValue("format").AsInt();
            if (SUPPORTED_VERSIONS.IndexOf(loc4) == -1)
            {
                Debug.Fail("ModelData [" + this.name + "] has an unsupported format - " + loc4 + " != " + VERSION);
                throw new ArgumentException("Unsupported file format!");
            }
            outputter.WriteToOutput("ModelData [" + this.name + "] has a supported format - " + loc4 + ".");
            name = loc3.GetValue("source").AsString();
            if (param1.ContainsKey("skeleton"))
            {
                Debug.WriteLine("ModelData [" + this.name + "], reading skeleton");
                outputter.WriteToOutput("ModelData [" + this.name + "] has a skeleton, reading skeleton.");
                skeleton = this.ReadSkeleton(param1["skeleton"], FileUtils.ReadTransformPool(param1["transform"]));
            }
            BKVTable loc5 = loc3.GetValue("materials").AsTable();
            textures = new List<PackedTexture>();
            if (loc5.HasValue("textures"))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "] has materials, reading textures.");
                loc8 = (int)(loc6 = loc5.GetValue("textures").AsTable()).GetNumValues();
                loc7 = 0;
                while (loc7 < loc8)
                {
                    Debug.WriteLine("ModelData [" + this.name + "], reading materials - " + loc7);
                    loc9 = loc6.GetValue(loc7).AsTable();
                    AddTexture(ReadTexture(loc9.GetValue("name").AsString(), param1, loc9));
                    loc7++;
                }
            }
            loc8 = (int)(loc6 = loc5.GetValue("groups").AsTable()).GetNumValues();
            materials = new List<Material>();
            loc7 = 0;
            while (loc7 < loc8)
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: textures read, reading material groups.");
                AddMaterial(new Material(loc6.GetValue(loc7).AsTable()));
                loc7++;
            }
            if (loc3.HasValue("blendShapes"))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "] has blend shapes, reading blendShapes.");
                loc8 = (int)(loc6 = loc3.GetValue("blendShapes").AsTable()).GetNumValues();
                blendShapes = new List<BlendShapeData>(loc8);
                loc7 = 0;
                while (loc7 < loc8)
                {
                    Debug.WriteLine("ModelData [" + this.name + "], reading blendShapes - " + loc7);
                    blendShapes[loc7] = new BlendShapeData(loc6.GetValue(loc7).AsTable());
                    loc7++;
                }
            }
            if (loc3.HasValue("lods"))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "] has LODs, reading LODs.");
                loc8 = (int)(loc6 = loc3.GetValue("lods").AsTable()).GetNumValues();
                lods = new List<LodData>(loc8);
                loc7 = 0;
                while (loc7 < loc8)
                {
                    Debug.WriteLine("ModelData [" + this.name + "], reading lods - " + loc7);
                    lods[loc7] = new LodData(loc6.GetValue(loc7).AsTable());
                    loc7++;
                }
            }
            loc8 = (int)(loc6 = loc3.GetValue("vertexDatas").AsTable()).GetNumValues();
            vertexDatas = new List<VertexData>();
            loc7 = 0;
            while (loc7 < loc8)
            {
                outputter.WriteToOutput("ModelData [" + this.name + "] has vertex data, reading vertexDatas.");
                Debug.WriteLine("ModelData [" + this.name + "], reading vertexDatas - " + loc7);
                AddVertexData(ReadVertexData(param1, loc6.GetValue(loc7).AsTable(), loc6));
                loc7++;
            }
            loc8 = (int)(loc6 = loc3.GetValue("meshes").AsTable()).GetNumValues();
            meshes = new List<MeshData>();
            indices = new List<ByteArray>();
            loc7 = 0;
            while (loc7 < loc8)
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading meshes & indices.");
                Debug.WriteLine("ModelData [" + this.name + "], reading meshes & indices - " + loc7);
                loc11 = (loc10 = loc6.GetValue(loc7).AsTable()).GetValue("name").AsString();
                if (IsBlendShapeBase(loc11))
                {
                    loc12 = new BlendShapeBaseData(loc11);
                    foreach (BlendShapeData loc13 in blendShapes)
                    {
                        if (loc13._base == loc11)
                        {
                            (loc12 as BlendShapeBaseData).blendShapes.Add(loc13);
                        }
                    }
                }
                else
                {
                    loc12 = new MeshData(loc10);
                }
                if (loc12.renderable)
                {
                    outputter.WriteToOutput("ModelData [" + this.name + "]: Reading indices from index-" + loc12.id);
                    Debug.WriteLine("ModelData [" + this.name + "], meshes & indices - " + loc7 + ", reading indices from index-" + loc12.id);
                    loc12.indices = ReadIndices(param1, "index-" + loc12.id);
                    loc12.indicesAmount = tempIndiceAmount;
                    indices.Add(loc12.indices);
                }
                if (param1.ContainsKey("mjoint-" + loc12.id))
                {
                    outputter.WriteToOutput("ModelData [" + this.name + "]: Reading mjoint-" + loc12.id);
                    Debug.WriteLine("ModelData [" + this.name + "], meshes & indices - " + loc7 + ", reading mjoint-" + loc12.id);
                    loc12.joints = FileUtils.ReadVectorOfInt(param1["mjoint-" + loc12.id]);
                }
                if (IsLODTarget(loc11))
                {
                    loc12.hasLOD = true;
                }
                AddMesh(loc12);
                loc7++;
            }
            if (loadCount == 0)
            {
                 Debug.WriteLine("Event has completed!");
            }
            outputter.WriteToOutput("ModelData: LoadBKV has returned true!");
            return true;
        }
        #endregion

        #region Vertex, Mesh & Skeleton Data Events
        public int AddVertexData(VertexData param1)
        {
            if (param1.id != -1)
            {
                vertexDatas.Add(param1);
            }
            else
            {
                param1.id = vertexDatas.Count;
                vertexDatas.Add(param1);
            }
            if (param1.weights != null && param1.joints != null)
            {
                param1.hasAnimationData = true;
            }
            outputter.WriteToOutput("ModelData: VertexData created. ID - " + param1.id);
            outputter.WriteToOutput("VertexData [" + param1.id + "], Number of Vertices - " + param1.numVertices + ", Normals? - " + (param1.normals != null).ToString() + ", Tangents? - " + (param1.tangents != null).ToString() + ", VColors? - " + (param1.vcolors != null).ToString() + ", Weights? - " + (param1.weights != null).ToString() + ", Joints? - " + (param1.joints != null).ToString() + ", Has Animation Data? - " + param1.hasAnimationData.ToString());
            outputter.WriteToOutput("VertexData [" + param1.id + "], Vertices ID - " + param1.verticesId + ", Normals ID - " + param1.normalsId + ", Tangents ID - " + param1.tangentsId + ", VColors ID - " + param1.vcolorsId + ", Weights ID - " + param1.weightsId + ", Joints ID - " + param1.jointsId);
            return (int)param1.id;
        }

        private VertexData ReadVertexData(Dictionary<string, ByteArray> param1, BKVTable param2, BKVTable param3)
        {
            BKVTable? loc6 = null, loc9 = null;
            int loc7 = 0, loc8 = 0, loc4 = param2.GetValue("id").AsInt();
            string loc10 = null, loc11 = null;
            TextureCoordinateData loc12 = null;
            VertexData loc5;
            (loc5 = new VertexData()).id = loc4;
            outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, vertex-" + loc4);
            loc5.vertices = FileUtils.ReadVectorOfNumbers(param1["vertex-" + loc4], (int)FileUtils.Encoding.ENCODING_NONE);
            if (loc5.vertices == null)
            {
                Debug.Fail("Failed to read vertices!");
            }
            if (param1.ContainsKey("normal-" + loc4))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, normal-" + loc4);
                loc5.normals = FileUtils.ReadVectorOfNumbers(param1["normal-" + loc4]);
            }
            if (param1.ContainsKey("tangent-" + loc4))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, tangent-" + loc4);
                loc5.tangents = FileUtils.ReadVectorOfNumbers(param1["tangent-" + loc4]);
            }
            if (param1.ContainsKey("vcolor-" + loc4))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, vcolor-" + loc4);
                loc5.vcolors = FileUtils.ReadVectorOfNumbers(param1["vcolor-" + loc4]);
            }
            loc5.texCoords = new Dictionary<string, TextureCoordinateData>();
            loc5.textures = new Dictionary<string, string>();
            if (param2.HasValue("uvs"))
            {
                loc7 = (int)(loc6 = param2.GetValue("uvs").AsTable()).GetNumValues();
                loc8 = 0;
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, reading UV maps. Data amount: " + loc7);
                while (loc8 < loc7)
                {
                    loc10 = (loc9 = loc6.GetValue(loc8).AsTable()).GetValue("channel").AsString();
                    if (loc9.GetValue("mapped").AsBool())
                    {
                        loc11 = loc9.GetValue("name").AsString();
                        if (!loc5.texCoords.ContainsKey(loc11))
                        {
                            outputter.WriteToOutput("ModelData [" + this.name + "], UV Map [" + loc11 + "], Reading texCoords for channel [" + loc10 + "]...");
                            loc5.texCoords.Add(loc11, new TextureCoordinateData(loc11, FileUtils.ReadVectorOfNumbers(param1[loc11], -1, 2), 0));
                            loc12 = loc5.texCoords[loc11];
                        }
                        loc12.channels.Insert(loc9.GetValue("components").AsString() == "xy" ? 0 : 1, loc10);
                        loc12.size += 2;
                    }
                    loc5.textures[loc10] = loc9.GetValue("texture").AsString();
                    outputter.WriteToOutput("ModelData [" + this.name + "], UV Map [" + loc11 + "], UV Map ID - " + loc8 + ", Is Mapped? - " + loc9.GetValue("mapped").AsBool().ToString() + ", Texture - " + loc5.textures[loc10]);
                    loc8++;
                }
            }
            if(param1.ContainsKey("weight-" + loc4))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, weight-" + loc4);
                loc5.weights = FileUtils.ReadVectorOfNumbers(param1["weight-" + loc4]);
            }
            if (param1.ContainsKey("joint-" + loc4))
            {
                outputter.WriteToOutput("ModelData [" + this.name + "]: Reading VertexData, joint-" + loc4);
                loc5.joints = FileUtils.ReadVectorOfNumbers(param1["joint-" + loc4], (int)FileUtils.Encoding.UNENCODED_BYTE);
            }
            outputter.WriteToOutput("ModelData [" + this.name + "]: Finished reading VertexData.");
            return loc5;
        }

        private List<JointData> ReadSkeleton(ByteArray param1, List<AlpineTransform> param2)
        {
            outputter.WriteToOutput("ModelData [" + this.name + "]: Reading Skeleton Joints.");
            Debug.WriteLine("ModelData [" + this.name + "]: Reading Skeleton Joints.");
            JointData loc5 = null;
            uint loc6 = 0, loc3 = param1.ReadUnsignedByte();
            int loc8 = 0, loc7 = 0;
            List<JointData> loc4 = new List<JointData>((int)loc3);
            outputter.WriteToOutput("ModelData [" + this.name + "] Skeleton Joint Found:");
            Debug.WriteLine("ModelData [" + this.name + "] Skeleton Joint Found:");
            while (loc7 < loc3)
            {
                (loc5 = new JointData()).id = loc7;
                loc5.name = param1.ReadUTF();
                loc5.isHardpoint = loc5.name.IndexOf("hp") == 0;
                loc5.parentId = param1.ReadUnsignedByte();
                if ((loc6 = param1.ReadUnsignedByte()) > 0)
                {
                    loc5.childrenIds = new List<uint>((int)loc6);
                    loc8 = 0;
                    while(loc8 < loc6)
                    {
                        loc5.childrenIds.Insert(loc8, param1.ReadUnsignedByte());
                        loc8++;
                    }
                }
                loc5.transformId = param1.ReadUnsignedShort();
                if (loc5.transformId < JointData.INVALID_TRANSFORMID)
                {
                    loc5.transform = param2[(int)loc5.transformId];
                }
                else
                {
                    Debug.Fail(loc5.name + " - Invalid Transform");
                    loc5.transform = new AlpineTransform();
                }
                loc5.invTransformId = param1.ReadUnsignedShort();
                if (loc5.invTransformId < JointData.INVALID_TRANSFORMID)
                {
                    loc5.invTransform = param2[(int)loc5.invTransformId];
                }
                else if (!loc5.isHardpoint)
                {
                    Debug.Fail(loc5.name + " - Invalid InvTransform");
                    loc5.invTransform = new AlpineTransform();
                }
                Debug.WriteLine("Joint ID: " + loc7 + ", Name: " + loc5.name + ", Is Hardpoint: " + loc5.isHardpoint + ", Parent ID: " + loc5.parentId + ", Transform ID: " + loc5.transformId + ", Inv Transform ID: " + loc5.invTransformId);
                outputter.WriteToOutput("Joint ID: " + loc7 + ", Name: " + loc5.name + ", Is Hardpoint: " + loc5.isHardpoint + ", Parent ID: " + loc5.parentId, "Transform ID: " + loc5.transformId + ", Transform Data: " + loc5.transform.ToString() + ", Inv Transform ID: " + loc5.invTransformId +  ", Inv Transform Data: " + loc5.invTransform.ToString());
                loc4.Insert(loc7, loc5);
                loc7++;
            }
            return loc4;
        }

        public int AddMesh(MeshData param1)
        {
            if (param1.vertexDataId > -1 && param1.vertexDataId < vertexDatas.Count)
            {
                param1.vertexData = vertexDatas[param1.vertexDataId];
            }
            if (param1.materialId > -1 && param1.materialId < materials.Count)
            {
                param1.material = materials[param1.materialId];
            }
            if (param1.indices != null)
            {
                indices.Add(param1.indices);
            }
            meshes.Add(param1);
            return meshes.Count - 1;
        }

        private ByteArray ReadIndices(Dictionary<string, ByteArray> param1, string param2)
        {
            Debug.WriteLine("Reading Indice - " + param2);
            uint loc10 = 0, loc11 = 0, loc12 = 0;
            ByteArray loc3 = param1[param2];
            if (loc3 == null)
            {
                Debug.WriteLine("Failed to read [" + param2 + "]");
                return null;
            }
            dynamic loc4 = loc3.ReadUnsignedByte() == 1;
            uint loc5 = (loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort());
            outputter.WriteToOutput("Creating Indice ByteArray. Size [" + (loc5 * 2).ToString() + "]");
            ByteArray loc6 = AlpineUtils.CreateByteArray(loc5 * 2);
            dynamic loc7 = loc3.ReadByte() > 0;
            int loc8 = 0, loc9 = 0;
            List<short> uints = new List<short>();
            int originalPos = 0;
            if (!loc7)
            {
                Debug.Write("loc3.ReadByte() is above 0, reading indices...");
                loc8 = 0;
                while (loc8 < loc5)
                {
                    originalPos = loc6.Position;
                    loc6.WriteShort(loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort());
                    loc6.Position = originalPos;
                    uints.Add(loc6.ReadShort());
                    loc8++;
                }
                uints.TrimExcess();
                tempIndiceAmount = uints.Count;
                return loc6;
            }
            Debug.Write("loc3.ReadByte() is exactly 0, reading indices...");
            while (loc8 < loc5)
            {
                if ((loc10 = loc3.ReadUnsignedByte()) == 0)
                {
                    loc12 = loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort();
                    loc9 = 0;
                    while (loc9 < loc12)
                    {
                        originalPos = loc6.Position;
                        loc6.WriteShort(loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort());
                        loc6.Position = originalPos;
                        uints.Add(loc6.ReadShort());
                        loc9++;
                    }
                }
                else
                {
                    loc11 = loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort();
                    loc12 = loc4 ? loc3.ReadUnsignedInt() : loc3.ReadUnsignedShort();
                    loc9 = 0;
                    while (loc9 < loc12)
                    {
                        Debug.WriteLine("Writing short: " + loc11 + loc9);
                        originalPos = loc6.Position;
                        loc6.WriteShort(loc11 + loc9);
                        loc6.Position = originalPos;
                        uints.Add(loc6.ReadShort());
                        loc9++;
                    }
                }
                loc8 += (int)loc12;
            }
            uints.TrimExcess();
            tempIndiceAmount = uints.Count;
            outputter.WriteToOutput("Read Indices: <" + string.Join(", ", uints) + ">");
            Debug.WriteLine("uint count: " + uints.Count + ", loc5: " + loc5);
            return loc6;
        }
        #endregion

        #region Textures & Material Data Events
        public int AddMaterial(Material param1)
        {
            if (param1.Id == -1)
            {
                materials.Add(param1);
                return materials.Count - 1;
            }
            if (materials.Count < param1.Id)
            {
                materials.Capacity = param1.Id + 1;
            }
            materials.Insert(param1.Id, param1);
            outputter.WriteToOutput("Material [" + param1.Id + "], Has Lighting Inputs? - " + param1.HasLightingInputs);
            return param1.Id;
        }

        public int AddTexture(PackedTexture param1)
        {
            outputter.WriteToOutput("ModelData [" + this.name + "]: Adding Texture - " + param1.Name);
            textures.Add(param1);
            return textures.Count - 1;
        }

        public PackedTexture ReadTexture(string param1, Dictionary<string, ByteArray> param2, BKVTable param3)
        {
            DrawImage loc8;
            BKVReader loc4;
            (loc4 = new BKVReader()).Load(param2[param1 + ".bkv"]);
            PackedTexture loc5 = new PackedTexture(param1, loc4.GetRoot());
            ByteArray loc6;
            string magic = (loc6 = param2[param1]).ReadUTFBytes(3);
            bool loc7 = magic == "ATF";
            Stream imageStream = new MemoryStream();
            imageStream.Write(loc6.data.ToArray());
            loc6.Position = 0;
            if (loc7)
            {
                loc5.RawData = loc6;
                loc5.Extension = ".atf";
                outputter.WriteToOutput("Texture [" + loc5.Name + ", Texture ID - " + loc5.Id + ", Downsample16BPP? - " + loc5.downsample16BPP + "]");
            }
            else
            {
                loadCount++;
                Debug.WriteLine("Image Extension: " + magic);
                switch (magic)
                {
                    case "�PN":
                        loc8 = DrawImage.FromStream(imageStream);
                        outputter.WriteToOutput("Texture [" + loc5.Name + ", Texture ID - " + loc5.Id + ", Downsample16BPP? - " + loc5.downsample16BPP + ", Width: " + loc8.Width + ", Height: " + loc8.Height + "]");
                        loc5.Texture = loc8;
                        loc5.Extension = ".png";
                        break;
                    case "PVR":
                        loc5.Extension = ".pvr";
                        break;
                    default:
                        loc5.Extension = "";
                        break;
                }
                loc5.RawData = loc6;
            }
            imageStream.Flush();
            imageStream.Close();
            return loc5;
        }

        public PackedTexture GetPackedTexture(string param1)
        {
            foreach (PackedTexture loc2 in this.textures)
            {
                if (loc2.Name == param1)
                {
                    return loc2;
                }
            }
            return null;
        }

        public List<string> GetPackedTextureNames
        {
            get
            {
                List<string> loc1 = new List<string>();
                foreach (PackedTexture loc2 in this.textures)
                {
                    loc1.Add(loc2.Name);
                }
                return loc1;
            }
        }

        public List<string> GetAllTextureNames
        {
            get
            {
                List<string> loc1 = new List<string>();
                int i = 0;
                foreach (PackedTexture loc2 in textures)
                {
                    loc1.AddRange(loc2.ImageNames);
                }
                return loc1;
            }
        }

        public Material FindCompatibleMaterial(Material param1)
        {
            foreach (Material loc2 in materials)
            {
                if (loc2.Equals(param1))
                {
                    return loc2;
                }
            }
            return null;
        }

        public PackedTexture FindCompatibleTexture(PackedTexture param1)
        {
            foreach (PackedTexture loc2 in textures)
            {
                if (loc2.Equals(param1))
                {
                    return loc2;
                }
            }
            return null;
        }
        #endregion

        #region BlendShapes & LOD Data Events
        public bool hasSkeletonData
        {
            get { return this.skeleton != null && this.skeleton.Count > 0; }
        }

        public bool IsBlendShapeBase(string param1)
        {
            if (blendShapes != null)
            {
                foreach (BlendShapeData loc2 in this.blendShapes)
                {
                    if (loc2._base == param1)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public BlendShapeData GetBlendShapeByName(string param1)
        {
            foreach (BlendShapeData loc2 in this.blendShapes)
            {
                if (loc2._base == param1)
                {
                    return loc2;
                }
            }
            return null;
        }

        public bool IsLODTarget(string param1)
        {
            List<LodData> loc2 = null;
            List<LodLevelData> loc4 = null;
            int loc5 = 0;
            LodLevelData loc6 = null;
            List<string> loc7 = null;
            int loc8 = 0, loc9 = 0, loc10 = 0, loc11 = 0;
            if (param1 == null || param1 == string.Empty)
            {
                return false;
            }
            if (lods != null)
            {
                loc2 = this.lods;
            }
            else
            {
                loc2 = new List<LodData>(0);
            }
            int loc3 = loc2.Count;
            loc9 = 0;
            while (loc9 < loc3)
            {
                loc5 = (loc4 = loc2[loc9].levels).Count;
                loc10 = 0;
                while (loc10 < loc5)
                {
                    loc8 = (loc7 = loc4[loc10].targets).Count;
                    loc11 = 0;
                    while (loc11 < loc8)
                    {
                        if (loc7[loc11] == param1)
                        {
                            return true;
                        }
                        loc11++;
                    }
                    loc10++;
                }
                loc9++;
            }
            return false;
        }
        #endregion

        #region Other Misc. Events
        public bool Downsample16BPP
        {
            get { return downsample16BPP; }
            set
            {
                downsample16BPP = value;
                foreach (PackedTexture loc2 in textures)
                {
                    loc2.downsample16BPP = value;
                }
            }
        }
        #endregion
    }
}
