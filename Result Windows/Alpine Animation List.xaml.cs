using Alpine.Renderables.Models.Animation.Data;
using Alpine.Renderables.Models.Skeleton;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Multi_Tool.Result_Windows
{
    /// <summary>
    /// Interaction logic for Alpine_Animation_List.xaml
    /// </summary>
    public partial class Alpine_Animation_List : Window
    {
        AnimationData animationData;
        string exportPath = string.Empty;
        public Alpine_Animation_List(AnimationData data = null)
        {
            InitializeComponent();
            if(data != null )
            {
                animationData = data;
                BuildList();
            }
            this.Show();
        }

        public void BuildList()
        {
            TreeViewItem root = new TreeViewItem();
            root.Header = animationData.name;
            ListViewItem rootItem = new ListViewItem();
            rootItem.Content = string.Format("Num. of Frames - {0}", animationData.numFrames);
            root.Items.Add(rootItem);
            rootItem = new ListViewItem();
            rootItem.Content = string.Format("Frames Per Second - {0}", animationData.fps);
            root.Items.Add(rootItem);
            rootItem = new ListViewItem();
            rootItem.Content = string.Format("Animation Time Length - {0}", animationData.Length);
            root.Items.Add(rootItem);
            if (animationData.HasSkeletalAnimation)
            {
                TreeViewItem treeSklelAnim = new TreeViewItem();
                treeSklelAnim.Header = "Skeltal Animation";
                // Handling Frame Data
                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = "Frame Data";
                for (int i = 0; i < animationData.skeletalAnimation.frameData.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = string.Format("Frame {0}: Time - {1}, Interpolated? - {2}.", 
                        animationData.skeletalAnimation.frameData[i].frame,
                        animationData.skeletalAnimation.frameData[i].time.ToString(),
                        animationData.skeletalAnimation.frameData[i].interpolate == true ? "Yes" : "No");
                    treeItem.Items.Add(item);
                }
                treeSklelAnim.Items.Add(treeItem);
                // Handling Joint Data
                treeItem = new TreeViewItem();
                treeItem.Header = "Animation Joint Data";
                for (int i = 0; i < animationData.skeletalAnimation.joints.Count; i++)
                {
                    TreeViewItem treeJoint = new TreeViewItem();
                    treeJoint.Header = animationData.skeletalAnimation.joints[i].joint;
                    for (int k = 0; k < animationData.skeletalAnimation.joints[i].frames.Count; k++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Content = string.Format("Frame {0}: Position - {1}.",
                        animationData.skeletalAnimation.joints[i].frameIds[k],
                        animationData.skeletalAnimation.joints[i].frames[k].ToString());
                        treeJoint.Items.Add(item);
                    }
                    treeItem.Items.Add(treeJoint);
                }
                treeSklelAnim.Items.Add(treeItem);
                root.Items.Add(treeSklelAnim);
            }
            ModelTreeList.Items.Add(root);
        }

        public void WriteToXAF()
        {
            Mouse.OverrideCursor = Cursors.AppStarting;
            XDocument xDocument = new XDocument(
                new XElement("MaxAnimation", 
                new XAttribute("version", "1.0.0"), new XAttribute("date", DateTime.UtcNow.ToString())));
            XElement customData = new XElement("CustomData");
            xDocument.Root.Add(WriteSceneInfo(), customData);
            if (animationData.HasSkeletalAnimation)
            {
                for(int i = 0; i < animationData.skeletalAnimation.joints.Count; i++)
                {
                    xDocument.Root.Add(WriteNode(i));
                }
            }

            //Finished Writing The XAF
            Mouse.OverrideCursor = null;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(exportPath, settings))
            {
                xDocument.Save(xw);
            }
            Debug.WriteLine("Exported to " + exportPath);
        }

        private XElement WriteSceneInfo()
        {
            XElement sceneInfo = new XElement("SceneInfo");
            XAttribute fileName = new XAttribute("fileName", animationData.name);
            XAttribute startTick = new XAttribute("startTick", 0);
            XAttribute endTick = new XAttribute("endTick", animationData.Length);
            XAttribute frameRate = new XAttribute("frameRate", Math.Round(animationData.fps));
            XAttribute ticksPerFrame;
            if (animationData.HasSkeletalAnimation)
            {
                ticksPerFrame = new XAttribute("ticksPerFrame", animationData.skeletalAnimation.frameData[1].time);
            }
            else if(animationData.HasBlendShapeAnimations)
            {
                ticksPerFrame = new XAttribute("ticksPerFrame", animationData.blendShapeAnimations[0].frameData[1].time);
            }
            else if (animationData.HasUVAnimations)
            {
                ticksPerFrame = new XAttribute("ticksPerFrame", animationData.uvAnimations[0].frameData[1].time);
            }
            else
            {
                ticksPerFrame = new XAttribute("ticksPerFrame", 1);
            }
            sceneInfo.Add(fileName, startTick, endTick, frameRate, ticksPerFrame);
            return sceneInfo;
        }

        private XElement WriteNode(int index)
        {
            XElement node = new XElement("Node");
            XAttribute nodeName = new XAttribute("name", animationData.skeletalAnimation.joints[index].joint);
            XAttribute nodeParent = new XAttribute("parentNode", "");
            XAttribute parentIndex = new XAttribute("parentNodeIndex", 0);
            XAttribute numChildren = new XAttribute("numChildren", 0);
            node.Add(nodeName, nodeParent, parentIndex, numChildren);

            // Writing the Samples for the chosen Node
            XElement samples = new XElement("Samples", new XAttribute("count", animationData.numFrames));
            for (int i = 0; i < animationData.numFrames; i++)
            {
                XElement sample = new XElement("S");
                XAttribute time = new XAttribute("t", animationData.skeletalAnimation.frameData[i].time);
                XAttribute position = new XAttribute("v", animationData.skeletalAnimation.joints[index].frames[i].SimpleToString());
                sample.Add(time, position);
                samples.Add(sample);
            }
            node.Add(samples);
            node.Add(WriteNodeControlPos(index), WriteNodeControlRot(index));
            return node;
        }

        private XElement WriteNodeControlPos(int index)
        {
            XElement controller = new XElement("Controller");
            XAttribute name = new XAttribute("name", string.Format("{0} \\ Transformation \\ Position", animationData.skeletalAnimation.joints[index].joint));
            XAttribute classOf = new XAttribute("classOf", "Position XYZ");
            XAttribute numChild = new XAttribute("numChildren", 3);
            XAttribute filter = new XAttribute("filterType", "pos");
            controller.Add(name, classOf, numChild, filter);
            XElement samples = new XElement("Samples", new XAttribute("count", animationData.numFrames));
            for (int i = 0; i < animationData.numFrames; i++)
            {
                XElement sample = new XElement("P3Val");
                XAttribute time = new XAttribute("t", animationData.skeletalAnimation.frameData[i].time);
                XAttribute position = new XAttribute("v", animationData.skeletalAnimation.joints[index].frames[i].translation.XYZToString());
                sample.Add(time, position);
                samples.Add(sample);
            }
            controller.Add(samples);
            return controller;
        }

        private XElement WriteNodeControlRot(int index)
        {
            XElement controller = new XElement("Controller");
            XAttribute name = new XAttribute("name", string.Format("{0} \\ Transformation \\ Rotation", animationData.skeletalAnimation.joints[index].joint));
            XAttribute classOf = new XAttribute("classOf", "Euler XYZ");
            XAttribute numChild = new XAttribute("numChildren", 3);
            XAttribute filter = new XAttribute("filterType", "rot");
            XAttribute eulerOrder = new XAttribute("eulerOrder", "XYZ");
            controller.Add(name, classOf, numChild, filter);
            XElement samples = new XElement("Samples", new XAttribute("count", animationData.numFrames));
            for (int i = 0; i < animationData.numFrames; i++)
            {
                XElement sample = new XElement("RVal");
                XAttribute time = new XAttribute("t", animationData.skeletalAnimation.frameData[i].time);
                XAttribute rotation = new XAttribute("v", animationData.skeletalAnimation.joints[index].frames[i].rotation.XYZToString());
                sample.Add(time, rotation);
                samples.Add(sample);
            }
            controller.Add(samples);
            return controller;
        }

        private void SaveDialog()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save Model Data";
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "3DS Max Animation File (*.xaf)|*.xaf";
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

        private void ExportAnim_Click(object sender, RoutedEventArgs e)
        {
            SaveDialog();
            switch (System.IO.Path.GetExtension(exportPath))
            {
                case ".xaf":
                    WriteToXAF();
                    return;
                default:
                    return;
            }
        }
    }
}
