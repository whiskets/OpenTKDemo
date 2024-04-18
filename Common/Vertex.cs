using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK.Common
{
    [StructLayout(LayoutKind.Explicit,Size=88)]
    public struct Vertex
    {
        public const int MAX_BONE_INFLUENCE = 4;
        // position
        [FieldOffset(0)]
        public Vector3 Position = Vector3.Zero;

        // normal
        [FieldOffset(12)]
        public Vector3 Normal = Vector3.Zero;

        // texCoords
        [FieldOffset(24)]
        public Vector2 TexCoords = Vector2.Zero;

        // tangent
        [FieldOffset(32)]
        public Vector3 Tangent = Vector3.Zero;

        // bitangent
        [FieldOffset(44)]
        public Vector3 Bitangent = Vector3.Zero;

        //bone indexes which will influence this vertex
        [FieldOffset(56)]
        // public int[] m_BoneIDs = new int[MAX_BONE_INFLUENCE];
        // public float[] m_BoneIDs = new float[MAX_BONE_INFLUENCE];
        public Vector4 m_BoneIDs = new Vector4();

        //weights from each bone
        [FieldOffset(72)]
        public float[] m_Weights = new float[MAX_BONE_INFLUENCE];

        public Vertex() { }

        public float[] Dataes { 
            get {
                var result =new float[8];
                result[0] = Position.X; result[1]= Position.Y; result[2] = Position.Z;
                result[3] = Normal.X; result[4] = Normal.Y; result[5] = Normal.Z;
                result[6] = TexCoords.X; result[7] = TexCoords.Y;
                return result;
            } 
        }

        public static int Size {  
            get 
            {
                unsafe
                {
                    return sizeof(Vector3) * 4 + sizeof(Vector2) + sizeof(int) * 4 + sizeof(float) * 4;
                }
               
            } 
        }

    }

    public struct ModelTexture
    {
        public int id;
        public string type;
        public string path;
    }

    public  struct BoneInfo
    {
        /*id is index in finalBoneMatrices*/
        public int id;

        /*offset matrix transforms vertex from model space to bone space*/
        public Matrix4 offset;

    };
}
