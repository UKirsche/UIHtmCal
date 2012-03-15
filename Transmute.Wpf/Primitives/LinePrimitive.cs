using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primitives3D
{
    public class LinePrimitive
    {
        private VertexPositionColor[] vertices;
        private GraphicsDevice device;
        private BasicEffect basicEffect; 

        public LinePrimitive(GraphicsDevice graphicsDevice)
        {
            this.device = graphicsDevice;
            basicEffect = new BasicEffect(device);
        }

        public void SetUpVertices(Vector3 v1, Vector3 v2, Color color)
        {
            Vector3 v3 = new Vector3(v2.X-0.1f, v2.Y-0.1f, v2.Z-0.1f);

            vertices = new VertexPositionColor[3];

            vertices[0] = new VertexPositionColor(v1, color);
            vertices[1] = new VertexPositionColor(v2, color);
            vertices[2] = new VertexPositionColor(v3, color);
       
        }

        public void Draw(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);

            }
        } 


    }
}
