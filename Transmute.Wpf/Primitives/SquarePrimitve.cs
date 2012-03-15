using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primitives3D
{
    public class SquarePrimitve : GeometricPrimitive
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public SquarePrimitve(GraphicsDevice device)
        {
            //Square does have 4 vertices:
            Vector3 v1,v2,v3,v4;

            //Coordinates in world-space origin
            v1 = new Vector3(1, 0, 1);
            v2 = new Vector3(-1, 0, 1);
            v3 = new Vector3(-1, 0, -1);
            v4 = new Vector3(1, 0, -1);

            //Add Indices for index buffe
            AddIndex(0);
            AddIndex(1);
            AddIndex(2);
            AddIndex(2);
            AddIndex(3);
            AddIndex(0);

            //Normal for each vertice: Up-Vector
            Vector3 normal = new Vector3(0, 1, 0);
            AddVertex(v1, normal);
            AddVertex(v2, normal);
            AddVertex(v3, normal);
            AddVertex(v4, normal);

            //Initialize Buffers
            InitializePrimitive(device);
        }
    }
}
