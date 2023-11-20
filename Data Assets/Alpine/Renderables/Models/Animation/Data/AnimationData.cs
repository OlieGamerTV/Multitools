using Alpine.Util;
using Alpine.Geom;
using Multi_Tool.Tools;
using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;
using Multi_Tool.Utilities;
using System.Windows.Controls;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class AnimationData
    {
        public const int VERSION = 6;
        public static List<int> SUPPORTED_VERSIONS = new List<int> { 4, 5, 6 };
        public string name = "";
        public float fps;
        public uint numFrames;
        public SkeltalAnimationData skeletalAnimation;
        public List<BlendShapeTargetAnimationData> blendShapeTargetAnimations;
        public List<BlendShapeAnimationData> blendShapeAnimations;
        public List<UVAnimationData> uvAnimations;
        public List<VisibilityAnimationData> visibilityAnimations;
        public List<AlphaAnimationData> alphaAnimations;
        OutputLog outputter;

        public AnimationData(ListView list = null) : base()
        {
            outputter = new OutputLog(list);
        }

        public float Length
        {
            get { return numFrames / fps; }
        }

        public bool HasSkeletalAnimation
        {
            get { return !(skeletalAnimation == null); }
        }

        public bool HasBlendShapeAnimations
        {
            get { return !(blendShapeAnimations == null) && blendShapeAnimations.Count > 0; }
        }

        public bool HasUVAnimations
        {
            get { return !(uvAnimations == null) && uvAnimations.Count > 0; }
        }

        public bool Load(string arg1, ByteArray animFile, ByteArray arg3 = null)
        {
            outputter.WriteToOutput("AnimationData.load: Loading data...");
            if (arg1 == null || arg1 == string.Empty)
            {
                Debug.WriteLine("Invalid url / path: " + arg1);
                return false;
            }
            DPack dPack = new DPack(outputter.GetLog);
            Dictionary<string, ByteArray> loc4 = dPack.Unpack(animFile);
            if (loc4 == null || loc4.Count < 1)
            {
                Debug.WriteLine("Invalid Animation DPack [" + arg1 + "]");
                return false;
            }
            outputter.WriteToOutput("-----------------------------------", "AnimationData.Load: Main File Loaded.");
            BKVReader reader = new BKVReader(outputter.GetLog);
            if (reader.IsBKV(loc4["desc"]))
            {
                LoadBKV(loc4);
            }
            return false;
        }

        public bool LoadBKV(Dictionary<string, ByteArray> data)
        {
            BKVTable channels;
            BKVValue channel;
            BKVReader reader = new BKVReader(outputter.GetLog);
            outputter.WriteToOutput("Loading BKV for ModelData [" + name + "]...");
            if (!reader.Load(data["desc"]))
            {
                return false;
            }
            BKVTable root = reader.GetRoot();
            int version = root.GetValue("format").AsInt();
            if (SUPPORTED_VERSIONS.IndexOf(version) == -1)
            {
                Debug.WriteLine("AnimationData [" + this.name + "] has an unsupported format - " +version + " != " + VERSION);
                return false;
            }
            outputter.WriteToOutput("AnimationData [" + this.name + "] has a supported format - " + version + ".");
            try
            {
                name = root.GetValue("name").AsString();
                fps = root.GetValue("frameRate").AsFloat();
                numFrames = root.GetValue("numFrames").AsUInt();
                channels = root.GetValue("channels").AsTable();
                channel = channels.GetValue("skeleton");
                outputter.WriteToOutput("AnimationData [" + this.name + "], FPS - " + fps + ", Num. Frames - " + numFrames + ".");
                if (channel.IsValid())
                {
                    outputter.WriteToOutput("AnimationData [" + this.name + "], reading Skeletal Animation Data...", "-----------------------------------------");
                    this.skeletalAnimation = this.ReadSkeletalAnimationData(channel.AsTable(), data);
                }
                channel = channels.GetValue("blendshape");
                if (channel.IsValid())
                {
                    outputter.WriteToOutput("AnimationData [" + this.name + "], reading Blendshape Animation Data...", "-----------------------------------------");
                    this.blendShapeTargetAnimations = this.ReadBlendShapeAnimationDatas(channel.AsTable(), data);
                }
                channel = channels.GetValue("uv");
                if(channel.IsValid())
                {
                    outputter.WriteToOutput("AnimationData [" + this.name + "], reading UV Animation Data...", "-----------------------------------------");
                    this.uvAnimations = this.ReadUVAnimationDatas(channel.AsTable(), data);
                }
                channel = channels.GetValue("visibility");
                if (channel.IsValid())
                {
                    outputter.WriteToOutput("AnimationData [" + this.name + "], reading Visibility Animation Data...", "-----------------------------------------");
                    this.visibilityAnimations = this.ReadVisibilityAnimationDatas(channel.AsTable(), data);
                }
                channel = channels.GetValue("alpha");
                if (channel.IsValid())
                {
                    outputter.WriteToOutput("AnimationData [" + this.name + "], reading Alpha Animation Data...", "-----------------------------------------");
                    this.alphaAnimations = this.ReadAlphaAnimationDatas(channel.AsTable(), data);
                }
                outputter.WriteToOutput("AnimationData: LoadBKV has returned true!");
                return true;
            }
            catch (Exception e)
            {
                outputter.WriteToOutput("File is corrupt. Error: " + e);
                return false;
            }
        }

        private SkeltalAnimationData ReadSkeletalAnimationData(BKVTable arg1, Dictionary<string, ByteArray> arg2)
        {
            BKVTable loc11;
            JointAnimationData loc12;
            int loc13;
            uint loc14;
            SkeltalAnimationData loc3 = new SkeltalAnimationData();
            loc3.transforms = FileUtils.ReadTransformPool(arg2["transform"]);
            loc3.precomputed = arg1.GetValue("precomputed").AsBool();
            BKVTable loc4 = arg1.GetValue("keys").AsTable();
            loc3.ReadFrames(loc4);
            ByteArray loc5 = arg2["joint"];
            BKVTable loc6 = arg1.GetValue("targets").AsTable();
            int loc7 = (int)loc6.GetNumValues(), loc8 = (int)loc4.GetNumValues();
            List<JointAnimationData> loc9 = new List<JointAnimationData>(loc7);
            int loc10 = 0;
            ListViewItem item = new ListViewItem();
            item.Content += "ReadSkeletalAnimationData: ";
            while (loc10 < loc7)
            {
                loc11 = loc6.GetValue(loc10).AsTable();
                loc12 = new JointAnimationData();
                loc12.joint = loc11.GetValue("joint").AsString();
                loc12.frames = new List<AlpineTransform>(loc8);
                loc12.frameIds = new List<uint>(loc8);
                loc13 = 0;
                item.Content += "Joint [" + loc10 + ", " + loc12.joint + "], Frames: ";
                while (loc13 < loc8)
                {
                    loc14 = loc5.ReadUnsignedShort();
                    loc12.frameIds.Insert(loc13, loc14);
                    loc12.frames.Insert(loc13, loc3.transforms[(int)loc14]);
                    item.Content += "[ ID: " + loc14 + ", Frame: " + loc3.transforms[(int)loc14].ToString() + " ] ";
                    loc13++;
                }
                loc9.Insert(loc10, loc12);
                outputter.WriteToOutput(item);
                item = new ListViewItem();
                loc10++;
            }
            loc3.joints = loc9;
            outputter.WriteToOutput("ReadUVAnimationDatas: Finished reading Skeletal Animation Data.");
            return loc3;
        }

        private List<BlendShapeTargetAnimationData> ReadBlendShapeAnimationDatas(BKVTable arg1, Dictionary<string, ByteArray> arg2)
        {
            BKVTable loc6, loc8;
            int loc9, loc10, loc3 = (int)arg1.GetNumValues(), loc5 = 0;
            List<BlendShapeTargetAnimationData> loc4 = new List<BlendShapeTargetAnimationData>();
            while (loc5 < loc3)
            {
                loc6 = arg1.GetValue(loc5).AsTable();
                loc4.Insert(loc5, new BlendShapeTargetAnimationData());
                loc4[loc5]._base = loc6.GetValue("base").AsString();
                loc4[loc5].target = loc6.GetValue("target").AsString();
                loc8 = loc6.GetValue("keys").AsTable();
                loc9 = (int)loc8.GetNumValues();
                loc4[loc5].ReadFrames(loc8);
                loc4[loc5].frames = new List<float>(loc9);
                loc10 = 0;
                while (loc10 < loc9)
                {
                    loc4[loc5].frames.Insert(loc10, loc8.GetValue(loc10).AsTable().GetValue("weight").AsFloat());
                    loc10++;
                }
                loc5++;
            }
            return loc4;
        }

        private List<UVAnimationData> ReadUVAnimationDatas(BKVTable arg1, Dictionary<string, ByteArray> arg2)
        {
            BKVTable loc6, loc8, loc11;
            int loc9, loc10, loc3 = (int)arg1.GetNumValues(), loc5 = 0;
            List<UVAnimationData> loc4 = new List<UVAnimationData>();
            ListViewItem item = new ListViewItem();
            item.Content += "ReadUVAnimationDatas: ";
            while (loc5 < loc3)
            {
                loc6 = arg1.GetValue(loc5).AsTable();
                loc4.Insert(loc5, new UVAnimationData());
                loc9 = (int)(loc8 = loc6.GetValue("keys").AsTable()).GetNumValues();
                loc4[loc5].ReadFrames(loc8);
                loc10 = 0;
                while(loc10 < loc9)
                {
                    if ((loc11 = loc8.GetValue(loc10).AsTable()).HasValue("texture"))
                    {
                        if (loc4[loc5].framesU == null)
                        {
                            loc4[loc5].framesU = new List<float>(loc9);
                            loc4[loc5].isOffsets = false;
                        }
                        loc4[loc5].framesU.Insert(loc10, loc11.GetValue("texture").AsInt());
                        item.Content += "Texture [" + loc4[loc5].framesU[loc10].ToString() + "] ";
                    }
                    else
                    {
                        if (loc4[loc5].framesU == null && loc4[loc5].framesV == null)
                        {
                            loc4[loc5].framesU = new List<float>(loc9);
                            loc4[loc5].framesV = new List<float>(loc9);
                            loc4[loc5].isOffsets = true;
                        }
                        loc4[loc5].framesU.Insert(loc10, loc11.GetValue("u").AsFloat());
                        loc4[loc5].framesV.Insert(loc10, loc11.GetValue("v").AsFloat());
                        item.Content += "Offset [ U: " + loc4[loc5].framesU[loc10].ToString() + ", V: " + loc4[loc5].framesV[loc10].ToString() + "] ";
                    }
                    loc10++;
                }
                loc4[loc5].texture = loc6.GetValue("targets").AsTable().GetValue(0).AsString();
                item.Content += "Target [" + loc4[loc5].texture + "] ";
                outputter.WriteToOutput(item);
                item = new ListViewItem();
                loc5++;
            }
            outputter.WriteToOutput("ReadUVAnimationDatas: Finished reading UV Animation Data.");
            return loc4;
        }

        private List<VisibilityAnimationData> ReadVisibilityAnimationDatas(BKVTable arg1, Dictionary<string, ByteArray> arg2)
        {
            BKVTable loc6, loc8, loc11;
            int loc9, loc10, loc3 = (int)arg1.GetNumValues(), loc5 = 0, loc12, loc13;
            List<VisibilityAnimationData> loc4 = new List<VisibilityAnimationData>();
            while (loc5 < loc3)
            {
                loc6 = arg1.GetValue(loc5).AsTable();
                loc4.Insert(loc5, new VisibilityAnimationData());
                loc8 = loc6.GetValue("keys").AsTable();
                loc9 = (int)loc8.GetNumValues();
                loc4[loc5].ReadFrames(loc8);
                loc4[loc5].frames = new List<bool>(loc9);
                loc10 = 0;
                while (loc10 < loc9)
                {
                    loc4[loc5].frames.Insert(loc10, loc8.GetValue(loc10).AsTable().GetValue("value").AsBool());
                    loc10++;
                }
                loc11 = loc6.GetValue("targets").AsTable();
                loc12 = (int)loc11.GetNumValues();
                loc4[loc5].targets = new List<string>(loc12);
                loc13 = 0;
                while (loc13 < loc12)
                {
                    loc4[loc5].targets.Insert(loc13, loc11.GetValue(loc13).AsString());
                    loc13++;
                }
                loc5++;
            }
            return loc4;
        }

        private List<AlphaAnimationData> ReadAlphaAnimationDatas(BKVTable arg1, Dictionary<string, ByteArray> arg2)
        {
            BKVTable loc6, loc8, loc11;
            int loc9, loc10, loc3 = (int)arg1.GetNumValues(), loc5 = 0, loc12, loc13;
            List<AlphaAnimationData> loc4 = new List<AlphaAnimationData>();
            while (loc5 < loc3)
            {
                loc6 = arg1.GetValue(loc5).AsTable();
                loc4.Insert(loc5, new AlphaAnimationData());
                loc8 = loc6.GetValue("keys").AsTable();
                loc9 = (int)loc8.GetNumValues();
                loc4[loc5].ReadFrames(loc8);
                loc4[loc5].frames = new List<float>(loc9);
                loc10 = 0;
                while (loc10 < loc9)
                {
                    loc4[loc5].frames.Insert(loc10, loc8.GetValue(loc10).AsTable().GetValue("value").AsFloat());
                    loc10++;
                }
                loc11 = loc6.GetValue("targets").AsTable();
                loc12 = (int)loc11.GetNumValues();
                loc4[loc5].targets = new List<string>(loc12);
                loc13 = 0;
                while (loc13 < loc12)
                {
                    loc4[loc5].targets.Insert(loc13, loc11.GetValue(loc13).AsString());
                    loc13++;
                }
                loc5++;
            }
            return loc4;
        }
    }
}
