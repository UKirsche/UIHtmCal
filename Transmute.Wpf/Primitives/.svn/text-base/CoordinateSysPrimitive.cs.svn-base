using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Transmute.Wpf;

namespace Primitives3D
{
    public class CoordinateSysPrimitive
    {
        private VertexPositionColor[] vertices; 
        private GraphicsDevice device; 
        private BasicEffect basicEffect; 

        public CoordinateSysPrimitive(GraphicsDevice device) 
        { 
            this.device = device; 
            basicEffect = new BasicEffect(device); 
            InitVertices(); 
        } 
 
        private void InitVertices() 
        { 
            vertices = new VertexPositionColor[30]; 
 
            //vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua); 
            //vertices[1] = new VertexPositionColor(Vector3.Right * 5, Color.Aqua); 
            //vertices[2] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Aqua); 
            //vertices[3] = new VertexPositionColor(new Vector3(4.5f, 0.5f, 0), Color.Aqua); 
            //vertices[4] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Aqua); 
            //vertices[5] = new VertexPositionColor(new Vector3(4.5f, -0.5f, 0), Color.Aqua); 
 
            //vertices[6] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua); 
            //vertices[7] = new VertexPositionColor(Vector3.Up * 5, Color.Aqua); 
            //vertices[8] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Aqua); 
            //vertices[9] = new VertexPositionColor(new Vector3(0.5f, 4.5f, 0), Color.Aqua); 
            //vertices[10] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Aqua); 
            //vertices[11] = new VertexPositionColor(new Vector3(-0.5f, 4.5f, 0), Color.Aqua); 
 
            //vertices[12] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua); 
            //vertices[13] = new VertexPositionColor(Vector3.Forward * 5, Color.Aqua); 
            //vertices[14] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Aqua); 
            //vertices[15] = new VertexPositionColor(new Vector3(0, 0.5f, -4.5f), Color.Aqua); 
            //vertices[16] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Aqua); 
            //vertices[17] = new VertexPositionColor(new Vector3(0, -0.5f, -4.5f), Color.Aqua); 

            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua);
            vertices[1] = new VertexPositionColor(Vector3.Right * 5, Color.Aqua);
            vertices[2] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Aqua);
            vertices[3] = new VertexPositionColor(new Vector3(4.5f, 0.5f, 0), Color.Aqua);
            vertices[4] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Aqua);
            vertices[5] = new VertexPositionColor(new Vector3(4.5f, -0.5f, 0), Color.Aqua);

            vertices[6] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua);
            vertices[7] = new VertexPositionColor(Vector3.Up * 5, Color.Aqua);
            vertices[8] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Aqua);
            vertices[9] = new VertexPositionColor(new Vector3(0.5f, 4.5f, 0), Color.Aqua);
            vertices[10] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Aqua);
            vertices[11] = new VertexPositionColor(new Vector3(-0.5f, 4.5f, 0), Color.Aqua);

            vertices[12] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Aqua);
            vertices[13] = new VertexPositionColor(Vector3.Forward * 5, Color.Aqua);
            vertices[14] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Aqua);
            vertices[15] = new VertexPositionColor(new Vector3(0, 0.5f, -4.5f), Color.Aqua);
            vertices[16] = new VertexPositionColor(new Vector3(0, 0, -5), Color.Aqua);
            vertices[17] = new VertexPositionColor(new Vector3(0, -0.5f, -4.5f), Color.Aqua); 
        } 
 
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix) 
        {
            basicEffect.World = Matrix.Identity;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 9);

            }
  
        } 
 
        public void DrawUsingPresetEffect() 
        { 
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 9);            
        } 
    }
}
