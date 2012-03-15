using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primitives3D
{
    public class PlanePrimitive
    {

        VertexBuffer planeVertexBuffer;
        GraphicsDevice device;
        BasicEffect basicEffect;

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public PlanePrimitive(GraphicsDevice graphicsDevice)
        {
            //LoadFloorPlan();
            device = graphicsDevice;
         
            // Create a BasicEffect, which will be used to render the primitive.
            basicEffect = new BasicEffect(device);
            basicEffect.EnableDefaultLighting();
        }

        /// <summary>
        /// Prepare vertices according to floorplan. 
        /// </summary>
        public void SetUpVertices(int[,] floorPlan)
        {
            int regionWidth = floorPlan.GetLength(0);
            int regionLength = floorPlan.GetLength(1);


            List<VertexPositionNormal> verticesList = new List<VertexPositionNormal>();
            for (int x = 0; x < regionWidth; x++)
            {
                for (int z = 0; z < regionLength; z++)
                {
                    if (floorPlan[x, z] == 0)
                    {
                        verticesList.Add(new VertexPositionNormal(new Vector3(x, 0, -z), new Vector3(0, 1, 0)));
                        verticesList.Add(new VertexPositionNormal(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0)));
                        verticesList.Add(new VertexPositionNormal(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0)));

                        verticesList.Add(new VertexPositionNormal(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0)));
                        verticesList.Add(new VertexPositionNormal(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 1, 0)));
                        verticesList.Add(new VertexPositionNormal(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0)));
                    }
                }
            }

            planeVertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormal), verticesList.Count, BufferUsage.WriteOnly);
            planeVertexBuffer.SetData<VertexPositionNormal>(verticesList.ToArray());
        }

        /// <summary>
        /// Draw Plane with relevant world and view matrix and color
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="color"></param>
        public void Draw(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Color color = Color.Aquamarine;
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / 255.0f;
            GraphicsDevice device = basicEffect.GraphicsDevice;
            device.DepthStencilState = DepthStencilState.Default;

            if (color.A < 255)
            {
                // Set renderstates for alpha blended rendering.
                device.BlendState = BlendState.AlphaBlend;
            }
            else
            {
                // Set renderstates for opaque rendering.
                device.BlendState = BlendState.Opaque;
            }

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(planeVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, planeVertexBuffer.VertexCount / 3);

            }
        } 
    }
}
