using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Genesis3D.Geometry.Math;

namespace Genesis3D.Geometry.Body
{
    public class SkinGeometry
    {
        short skinVertexCount = 0;
        List<SkinVertex> skinVertexArray;

        short normalCount = 0;
        List<GenVec3D> normalArray;

        short faceCount = 0;
        int faceListSize = 0;
        List<FaceType> faceList;

        List<GenVec3D> maxs, mins;

        public int meshAmount, indiceAmount;
        public Dictionary<int, List<Vector3>> indices, normals;

        public void ReadBodyGeo(Stream dataStream, int currentPos = -1)
        {
            BinaryReader br = new BinaryReader(dataStream);

            if(currentPos != -1) { br.BaseStream.Position = currentPos; }

            skinVertexCount = br.ReadInt16();
            skinVertexArray = new List<SkinVertex>(skinVertexCount);
            string skinVertexPrint = "Skin Vertices: \n";
            for(int i = 0; i < skinVertexCount; i++)
            {
                SkinVertex vertex = new SkinVertex()
                {
                    x = br.ReadInt32(), 
                    y = br.ReadInt32(), 
                    z = br.ReadInt32(), 
                    u = br.ReadInt32(), 
                    v = br.ReadInt32(),
                    refBoneIndex = br.ReadByte()
                };
                br.ReadBytes(3);
                skinVertexPrint += $"( X={vertex.x}, Y={vertex.y}, Z={vertex.z}, U={vertex.u}, V={vertex.v}, Bone Index={vertex.refBoneIndex} ) \n";
                skinVertexArray.Add(vertex);
            }
            Debug.WriteLine(skinVertexPrint);

            normalCount = br.ReadInt16();
            normalArray = new List<GenVec3D>(normalCount);

            for (int i = 0; i < normalCount; i++)
            {
                GenVec3D normal = new GenVec3D()
                {
                    x = br.ReadInt32(),
                    y = br.ReadInt32(),
                    z = br.ReadInt32()
                };
                br.ReadBytes(4);
                normalArray.Add(normal);
            }
        }

        public void ReadBodyGeo(BinaryReader br, int currentPos = -1)
        {
            if (currentPos != -1) { br.BaseStream.Position = currentPos; }

            skinVertexCount = br.ReadInt16();
            skinVertexArray = new List<SkinVertex>(skinVertexCount);
            string skinVertexPrint = "Skin Vertices: \n";
            for (int i = 0; i < skinVertexCount; i++)
            {
                SkinVertex vertex = new SkinVertex()
                {
                    x = br.ReadSingle(),
                    y = br.ReadSingle(),
                    z = br.ReadSingle(),
                    u = br.ReadSingle(),
                    v = br.ReadSingle(),
                    refBoneIndex = br.ReadByte()
                };
                br.ReadBytes(3);
                skinVertexPrint += $"( XYZ={vertex.x} {vertex.y} {vertex.z}, UV={vertex.u} {vertex.v}, Bone Index={vertex.refBoneIndex} ) \n";
                skinVertexArray.Add(vertex);
            }
            Debug.WriteLine(skinVertexPrint);

            normalCount = br.ReadInt16();
            normalArray = new List<GenVec3D>(normalCount);
            string normalPrint = "Normals: \n";

            for (int i = 0; i < normalCount; i++)
            {
                GenVec3D normal = new GenVec3D()
                {
                    x = br.ReadSingle(),
                    y = br.ReadSingle(),
                    z = br.ReadSingle()
                };
                br.ReadBytes(4);
                normalPrint += $"( XYZ={normal.x} {normal.y} {normal.z} ) \n";
                normalArray.Add(normal);
            }
            Debug.WriteLine(normalPrint);
        }

        public void ReadBodyIndices(BinaryReader br, int currentPos = -1)
        {
            if (currentPos != -1)
            {
                br.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }
            meshAmount = br.ReadInt32();
            indiceAmount = br.ReadInt32();

            indices = new Dictionary<int, List<Vector3>>();
            normals = new Dictionary<int, List<Vector3>>();
            string indicesPrint = "Indices: \n";
            string normalsPrint = "Normals Indices: \n";
            for (int i = 0; i < meshAmount; i++)
            {
                List<Vector3> indicesVec = new List<Vector3>(indiceAmount);
                List<Vector3> normalsVec = new List<Vector3>(indiceAmount);
                for (int j = 0; j < indiceAmount; j++)
                {
                    Vector3 normalVec = new Vector3()
                    {
                        X = br.ReadInt16(),
                        Y = br.ReadInt16(),
                        Z = br.ReadInt16()
                    };
                    Vector3 indiceVec = new Vector3()
                    {
                        X = br.ReadInt16(),
                        Y = br.ReadInt16(),
                        Z = br.ReadInt16()
                    };
                    br.ReadInt16();
                    indicesPrint += $"( {indiceVec.X} {indiceVec.Y} {indiceVec.Z} ) \n";
                    normalsPrint += $"( {normalVec.X} {normalVec.Y} {normalVec.Z} ) \n";
                    normalsVec.Add(normalVec);
                    indicesVec.Add(indiceVec);
                }
                indicesPrint += $"( Indices Amount: {indicesVec.Count}) \n";
                normalsPrint += $"( Normals Indices Amount: {normalsVec.Count}) \n";
                indices.Add(i, indicesVec);
                normals.Add(i, normalsVec);
            }
            Debug.WriteLine(indicesPrint);
            Debug.WriteLine(normalsPrint);
        }
    }

    public enum FaceType
    {
        GE_BODYINST_FACE_TRIANGLE,
        GE_BODYINST_FACE_TRISTRIP,
        GE_BODYINST_FACE_TRIFAN
    }
}
