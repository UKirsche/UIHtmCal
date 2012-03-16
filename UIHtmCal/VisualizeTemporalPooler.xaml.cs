using System;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Transmute.Wpf;
using Primitives3D;
using System.Windows.Controls.Primitives;
using HTM.HTMLibrary;
using HTM.HTMInterfaces;

namespace HTM.UIHtmCal
{
	

	struct HTMColorInformation
	{
		public Color htmColor;
		public string htmInformation;

		public HTMColorInformation(Color cColor, string cInformation)
		{
			htmColor = cColor;
			htmInformation = cInformation;
		}
	}

	class HTMOverViewInformation
	{

		public HTMOverViewInformation()
		{
			stepCount = "";
			chosenHTMElement = "Region";
			positionElement = "";
			activityRate = "";
			precisionRate = "";
		}

		public string stepCount;
		public string chosenHTMElement;
		public string positionElement;
		public string activityRate;
		public string precisionRate;
	}

	enum HTMColors
	{
		Learning,
		FalsePrediction,
		RightPrediction,
		Predicting,
		SequencePredicting,
		Active,
		Inactive,
		Selected,
		Inhibited
	}

	/// <summary>
	/// Interaktionslogik für VisualizeTemporalPooler.xaml
	/// </summary>
	public partial class VisualizeTemporalPooler : Window
	{
		const float ZHTMREGION = -5.0f;
		const float ZHTMPLANE = -7.5f;
		const float YHTMPLANE = -5.0f;
		
		#region Variables

		//Graphics Device
		GraphicsDevice device;

		//Spritefont

		//Primitives
		private CubePrimitive cube;
		private CoordinateSysPrimitive coordinateSystem;
		private PlanePrimitive inputPlane;
		private SquarePrimitve bit;
		private LinePrimitive connectionLine;

		//Global Matrices
		Matrix viewMatrix;
		Matrix projectionMatrix;

		//Camera start-off original position
		Vector3 cameraPositionOrigin = new Vector3(-20.0f, 15.0f, -5.0f);
		
		//Camera parameters for view matrix
		Vector3 lookPos = Vector3.Zero;
		Vector3 camup = new Vector3(0, 1, 0);
		Vector3 lookAt;

		Quaternion cameraRotation = Quaternion.Identity;

		//Colors
		Dictionary<HTMColors, HTMColorInformation> dictColors;

		//Right Legend-Elements
		HTMOverViewInformation rightLegend;

		// A yaw and pitch applied to the second viewport based on input
		private float yawHTM = 0f;
		private float pitchHTM = 0f;
		private float roll1 = 0f;
		private float zoom1 = 1f;

		private float yawCamera = 0f;
		private float pitchCamera = 0f;
		private float roll2 = 0f;
		private float zoom2 = 0f;

		private float time = 0f;
		private float timeElapsed = 0f;
		private float time2 = 0f;

		Color cubeColor = Color.Red;

		ToggleButton btnAnimate;

		bool contentLoaded = false;

		#endregion

		#region Constructor
		public VisualizeTemporalPooler()
		{
			InitializeComponent();

		}

		#endregion

		#region Properties

		/// <summary>
		/// reference to HTM Controler
		/// </summary>
		public HTMController Controler { get; set; }

		/// <summary>
		/// 2 dimensional Array to grab predition information
		/// </summary>
		public float[,] arrayPredictions { get; set; }

		/// <summary>
		/// Reference to HtmRegion
		/// </summary>
		public Region HtmRegion { get; set; }

		/// <summary>
		/// Reference to list of columsn from region for traversing
		/// </summary>
		public List<List<Column>> RegionColumnList { get; set; }

		#endregion

		#region LoadContent

		private void LoadContent(object sender, GraphicsDeviceEventArgs e)
		{
			if (!contentLoaded)
			{

				//Load Colors for net
				dictColors =  new Dictionary<HTMColors,HTMColorInformation>();
				dictColors.Add(HTMColors.Active, new HTMColorInformation(Color.Black, "Cell is activated"));
				dictColors.Add(HTMColors.Inactive, new HTMColorInformation(Color.White, "Cell is inactive"));
				dictColors.Add(HTMColors.Learning, new HTMColorInformation(Color.DarkGray, "Cell is learning"));
				dictColors.Add(HTMColors.Predicting, new HTMColorInformation(Color.Orange, "Cell is predicting"));
				dictColors.Add(HTMColors.SequencePredicting, new HTMColorInformation(Color.Violet, "Cell is sequence predicting"));
				dictColors.Add(HTMColors.RightPrediction, new HTMColorInformation(Color.Green, "Cell correctly predicted"));
				dictColors.Add(HTMColors.FalsePrediction, new HTMColorInformation(Color.Red, "Cell falsely predicted"));
				dictColors.Add(HTMColors.Selected, new HTMColorInformation(Color.Yellow, "Cell is selected"));
				dictColors.Add(HTMColors.Inhibited, new HTMColorInformation(Color.Khaki, "Column is inhibited"));

				//Create OverviewElement
				rightLegend = new HTMOverViewInformation();

				//Get referencees for traversing regions
				HtmRegion = Controler.HTMRegions[0] as Region;
				RegionColumnList = HtmRegion.columnGrid;

				//Prepare Array for 2-dim-content
				arrayPredictions = new float[HtmRegion.width, HtmRegion.height];

				

				//Device
				device = e.GraphicsDevice;
				
				//Prepare Cube
				cube = new CubePrimitive(device);
				coordinateSystem = new CoordinateSysPrimitive(device);
				inputPlane = new PlanePrimitive(device);
				bit = new SquarePrimitve(device);
				connectionLine = new LinePrimitive(device);

				//Prepare xnaControl
				xnaControl1.ClearColor = Color.CornflowerBlue;
				xnaControl1.ShowGrid = false;
				xnaControl1.ScrollBarsVisible = false;
				xnaControl1.ToolbarVisible = false;


				btnAnimate = new ToggleButton();
				btnAnimate.IsChecked = false;
				btnAnimate.Content = "Animate";

				SetUpCamera(e);

				contentLoaded = true;
			}
		}

		private void SetUpCamera(GraphicsDeviceEventArgs e)
		{
			//Look-At-Vector
			lookAt = lookPos - cameraPositionOrigin;
			lookAt.Normalize();

			projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
				e.GraphicsDevice.Viewport.AspectRatio, 1, 100);
		}

		#endregion

		#region Draw

		private void xnaControl1_Draw(object sender, GraphicsDeviceEventArgs e)
		{
			e.GraphicsDevice.Clear(xnaControl1.ClearColor);

			//draw Legend:
			DrawLegend();

			//Draw HTM
			DrawHTMRegion();     
			DrawHTMPlane();

			//Draw Prediction Plane
			DrawHTMRegionPrediction();


			//Draw CoordinateSystem:
			if (xnaControl1.showCoordinateSystem) 
			{

				coordinateSystem.Draw(viewMatrix, projectionMatrix);
			}
		}

	   

		#endregion

		#region Mouse Events

		/// <summary>
		/// Compute rotation angle for camera view and rotation angle for X-Y-Axis rotation for whole scene
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void xnaControl1_MouseMove(object sender, HwndMouseEventArgs e)
		{
			double diffX = e.Position.X - e.PreviousPosition.X;
			double diffY = e.Position.Y - e.PreviousPosition.Y;

			//Rotation angle for camera in world space
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				yawCamera += (float)(diffX) * .01f;
				pitchCamera += (float)(diffY) * .01f;
			}

			//Rotation angle for htm-objects in world space
			if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				yawHTM += (float)(diffX) * .01f;
				pitchHTM += (float)(diffY) * .01f;
			}
		}

		#endregion

		#region Update

		private void xnaControl1_Update(object sender, HwndUpdateEventArgs e)
		{
			xnaControl1.ParentWindow = this;


			timeElapsed = (float)e.GameTime.ElapsedGameTime.TotalSeconds;
			time += timeElapsed;

			//Zoom
			ProcessMouseZoom();
			UpdateCamera();

		}

		/// <summary>
		/// Move Camera according to rotation angle and zoom factor.
		/// </summary>
		private void UpdateCamera()
		{

			//speed factor for Zoom:
			float speed = 4.0f;

			//Zoom in and out
			Vector3 translatedOrigin = cameraPositionOrigin+(lookAt * xnaControl1.Zoom * speed);


			//Rotate camera around new translated position
			Matrix worldRotate = Matrix.CreateRotationX(pitchCamera) * Matrix.CreateRotationY(yawCamera);
			Vector3 cameraPosition = Vector3.Transform(translatedOrigin, worldRotate);

			//Create View matrix
			viewMatrix = Matrix.CreateLookAt(cameraPosition, lookPos, camup);
			
			
		}


		//
		private void ProcessMouseZoom()
		{
			MouseState mouse = Mouse.GetState();

			var wheel = xnaControl1.MouseWheelValue;
			if (wheel < 0)
			{
				ZoomOut(xnaControl1);
				xnaControl1.MouseWheelValue = 0;


			}
			else if (wheel > 0)
			{
				ZoomIn(xnaControl1);
				xnaControl1.MouseWheelValue = 0;
			}

			//if (mouse.MiddleButton == ButtonState.Pressed)
			//{
			//    ResetZoom(xnaControl1);
			//}
		}

   
		#endregion

		#region Button Events

		private void redButton_Click(object sender, RoutedEventArgs e)
		{
			cubeColor = Color.Red;
		}

		private void greenButton_Click(object sender, RoutedEventArgs e)
		{
			cubeColor = Color.Green;
		}

		private void blueButton_Click(object sender, RoutedEventArgs e)
		{
			cubeColor = Color.Blue;
		}

		#endregion

		#region ScrollBar Events

		void HScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			yawCamera = (float)e.NewValue;
		}

		void VScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			pitchCamera = (float)e.NewValue;
		}

		#endregion

		#region Zoom Methods

		public void ZoomOut(XnaViewer viewer, float distance = 0.1f)
		{
			viewer.Zoom -= distance;
		}

		public void ZoomIn(XnaViewer viewer, float distance = 0.1f)
		{
			viewer.Zoom += distance;
		}

		public void ResetZoom(XnaViewer viewer)
		{
			viewer.Zoom = 1f;
		}

		#endregion


		#region xna-methods

		/// <summary>
		/// Gets statistical info to fill HTMOverview for Legend
		/// </summary>
		/// <param name="statLayer"></param>
		private void FillHTMOveriew(IStatistics statLayer)
		{

			if (statLayer is Region)
			{
				//Set Legend-Information on the right:
				rightLegend.chosenHTMElement = "Region";
				rightLegend.positionElement = "";
			}
			else if (statLayer is Column)
			{
				Column col = statLayer as Column;
				//Set Legend-Information on the right:
				rightLegend.chosenHTMElement = "Column";
				rightLegend.positionElement = col.cPos.ToString();
			}
			else if (statLayer is Cell)
			{
				Cell cell = statLayer as Cell;
				rightLegend.chosenHTMElement = "Cell";
				rightLegend.positionElement = cell.column.cPos.ToString() + "-: " + cell.Index;
			}

			rightLegend.stepCount = statLayer.StepCounter.ToString();
			rightLegend.activityRate = statLayer.ActivityRate.ToString();
			rightLegend.precisionRate = statLayer.PredictPrecision.ToString();

		}

		/// <summary>
		/// Draws legend for HTM-Algorithm on the left and right side of the animation.
		/// </summary>
		private void DrawLegend()
		{
			int gridWidth = (int) xnaControl1.GridSize.X;
			int gridHeight = (int) xnaControl1.GridSize.Y;
			int gridHeightBuffer = 10;
			int gridWidthBuffer = 30;

			Vector2 startVectorLeft = new Vector2(20, 45);
			Vector2 startVectorRight = new Vector2(800, 45);
			Vector2 startVectorRightTab = new Vector2(920, 45);


			//Draw left legend
			SpriteBatch spriteBatch = xnaControl1.SpriteBatch;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null);
			spriteBatch.DrawString(xnaControl1.myFont, "Legend", startVectorLeft, Color.Black);

			foreach (KeyValuePair<HTMColors, HTMColorInformation> item in dictColors)
			{
				startVectorLeft.Y += gridHeight + gridHeightBuffer;
				spriteBatch.Draw(xnaControl1.Grid, new Rectangle((int)startVectorLeft.X, (int)startVectorLeft.Y,
					gridWidth, gridHeight), item.Value.htmColor);
				spriteBatch.DrawString(xnaControl1.myFont, item.Value.htmInformation, new Vector2(startVectorLeft.X + gridWidthBuffer, startVectorLeft.Y), 
					Color.White);
			}


			//Draw right legend
			spriteBatch.DrawString(xnaControl1.myFont, "HTM Information", startVectorRight, Color.Black);
			startVectorRight.Y += gridHeight + gridHeightBuffer + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer + gridHeightBuffer;
			spriteBatch.DrawString(xnaControl1.myFont, "Steps: ", startVectorRight, Color.White);
			spriteBatch.DrawString(xnaControl1.myFont, rightLegend.stepCount, startVectorRightTab, Color.White);
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			spriteBatch.DrawString(xnaControl1.myFont, "Chosen: ", startVectorRight, Color.White);
			spriteBatch.DrawString(xnaControl1.myFont, rightLegend.chosenHTMElement, startVectorRightTab, Color.White);
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			spriteBatch.DrawString(xnaControl1.myFont, "Position: ", startVectorRight, Color.White);
			spriteBatch.DrawString(xnaControl1.myFont, rightLegend.positionElement, startVectorRightTab, Color.White);
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			spriteBatch.DrawString(xnaControl1.myFont, "Activity Rate: ", startVectorRight, Color.White);
			spriteBatch.DrawString(xnaControl1.myFont, rightLegend.activityRate, startVectorRightTab, Color.White);
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			spriteBatch.DrawString(xnaControl1.myFont, "Precision: ", startVectorRight, Color.White);
			spriteBatch.DrawString(xnaControl1.myFont, rightLegend.precisionRate, startVectorRightTab, Color.White);


			spriteBatch.End();
		}


		/// <summary>
		/// Draws the region prediction as flat 2-d array
		/// </summary>
		private void DrawHTMRegionPrediction()
		{
			//Buffer 
			int gridHeightBuffer = 5;
			int gridWidthBuffer = 10;

			//StartVector
			Vector2 startVectorLeft = new Vector2(gridWidthBuffer, 550);

			  //Gridheight, -width for simple prediction visualization:
			int gridHeight = (int)xnaControl1.GridSize.X / 2;
			int gridWidth = (int)xnaControl1.GridSize.Y / 2;


			//Draw Prediction Legend
			SpriteBatch spriteBatch = xnaControl1.SpriteBatch;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null);
			spriteBatch.DrawString(xnaControl1.myFont, "Region Prediction", startVectorLeft, Color.Black);
			//Go one more line down
			startVectorLeft.Y += gridHeight + gridHeightBuffer;

			for (int i = 0; i < arrayPredictions.GetLength(0); i++)
			{
				//Go one more line down
				startVectorLeft.Y += gridHeight + gridHeightBuffer;
				for (int j = 0; j < arrayPredictions.GetLength(1); j++)
				{
					//Adapt color
					float multiPlic = 1- arrayPredictions[i,j];
					Color newColor = new Color((int)(multiPlic * 255), (int)(multiPlic * 255), (int)(multiPlic * 255));
					

					spriteBatch.Draw(xnaControl1.Grid, new Rectangle((int)startVectorLeft.X, (int)startVectorLeft.Y,
						gridWidth, gridHeight), newColor);

					startVectorLeft.X += gridWidth + gridWidthBuffer;
				}
				//CR
				startVectorLeft.X = gridWidthBuffer;
			}

			spriteBatch.End();
		}


		private void DrawHTMRegion()
		{
			Matrix worldTransZ = Matrix.CreateTranslation(new Vector3(0, 0, ZHTMREGION));
			Matrix worldTrans;
			Matrix worldScale;
			Matrix worldRotate = Matrix.CreateRotationX(pitchHTM) * Matrix.CreateRotationY(yawHTM);


			FillHTMOveriew(HtmRegion);


			foreach (List<Column> yCols in RegionColumnList)
			{
				foreach (Column col in yCols)
				{
					int colCounter = 0;

					foreach (Cell cell in col.cells)
					{
						Color color;
						float alphaValue;

						GetColorFromCell(cell, out color, out alphaValue);

						//Check for Cell-Selection
						if (col.IsDataGridSelected)
						{
							//Set Legend-Information on the right:
							FillHTMOveriew(col);

							alphaValue = 1.0f;
							color = dictColors[HTMColors.Selected].htmColor;
							worldScale = Matrix.CreateScale(new Vector3(0.5f, 0.5f, 0.5f));
						}
						else if (col.IsInhibited)
						{
							alphaValue = 1.0f;
							color = dictColors[HTMColors.Inhibited].htmColor;
							worldScale = Matrix.CreateScale(new Vector3(0.3f, 0.3f, 0.3f));
						}

						if (cell.IsDataGridSelected)
						{
							//Set Legend-Information on the right:
							FillHTMOveriew(cell);

							//Set Legend-Information

							alphaValue = 1.0f;
							worldScale = Matrix.CreateScale(new Vector3(0.7f, 0.7f, 0.7f));
						}
						else
						{
							//Set Legend-Information on the right:
							worldScale = Matrix.CreateScale(new Vector3(0.3f, 0.3f, 0.3f));
						}


						if (cell.isPredicting)
						{
							//Für RegionPrediction Visualization
							colCounter++;
						}

						Vector3 transVector = new Vector3(col.cPos.X ,  cell.Index , col.cPos.Y );
						worldTrans = Matrix.CreateTranslation(transVector) * worldTransZ;
						Matrix world = worldScale * worldTrans * worldRotate;

						//Draw Cube
						cube.Draw(world, viewMatrix, projectionMatrix, color, alphaValue);

						//From running through: draw synapse connections
						DrawDistalSynapseConnections(ref worldTransZ, ref worldRotate, col, cell);
					}

					//Send column indices with actual prediction value in 2-dimArray
					float result = (float)colCounter / (float)HtmRegion.cellsPerCol;
					arrayPredictions[col.cPos.X, col.cPos.Y] = result;

					//Draw proximal synapse connections:
					DrawProximalSynapseConnections(ref worldRotate, col);

					//Draw InputSpace
					DrawInputSpace(col);

				}
			}      
		}


		/// <summary>
		/// Helper Method to get color from cell activity
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="color"></param>
		/// <param name="alphaValue"></param>
		private void GetColorFromCell(Cell cell, out Color color, out float alphaValue)
		{
			color = dictColors[HTMColors.Inactive].htmColor;
			alphaValue = 1.0f;

			//if (cell.wasPredicted && cell.isActive)
			//{
			//    color = dictColors[HTMColors.RightPrediction].htmColor;
			//}

			//else if (cell.wasPredicted && !cell.isActive)
			//{
			//    color = dictColors[HTMColors.FalsePrediction].htmColor;
			//}

			//else if (cell.isPredicting)
			//{
			//    color = dictColors[HTMColors.Predicting].htmColor;
			//}
			//else if (cell.isLearning)
			//{
			//    color = dictColors[HTMColors.Learning].htmColor;
			//}
			//else if (cell.isActive)
			//{
			//    color = dictColors[HTMColors.Active].htmColor;
			//}

			if (cell.isPredicting && !cell.isSegmentPredicting)
			{
				color = dictColors[HTMColors.Predicting].htmColor;
			}
			else if (cell.isSegmentPredicting)
			{
				color = dictColors[HTMColors.SequencePredicting].htmColor;
			}
		}



		/// <summary>
		/// Draw distal synapse connections for chosen cells.
		/// Attention! lateral movement of planes, regions causes Z-Value correction
		/// </summary>
		/// <param name="worldTransUp"></param>
		/// <param name="worldRotate"></param>
		/// <param name="col"></param>
		/// <param name="cell"></param>
		private void DrawProximalSynapseConnections( ref Matrix worldRotate, Column col)
		{
			//Draw Connections if existing
			if (col.IsDataGridSelected || (xnaControl1.showSpatialLearning && col.IsActive))
			{
				foreach (Synapse syn in col.proximalSegment.synapses)
				{
					if (col.StepCounter > 0) 
					{ 

						ProximalSynapse pSyn = syn as ProximalSynapse;
						//Get the two Vectors to draw Line between -> Start Pos
						Vector3 startPos = new Vector3(col.cPos.X, 0, col.cPos.Y + ZHTMREGION);
						//Get InputSource-Pos:
						int x = pSyn.inputSource.ix;
						int y = -5;
						int z = pSyn.inputSource.iy;
						Vector3 endPos = new Vector3(x, y, z + ZHTMPLANE);


						//Check for color
						if (pSyn.isActive)
						{
							connectionLine.SetUpVertices(startPos, endPos, Color.Green);
						}
						else if (pSyn.isActiveNotConnected)
						{
							connectionLine.SetUpVertices(startPos, endPos, Color.Orange);
						}
						else if(!pSyn.isActiveNotConnected)
						{
							connectionLine.SetUpVertices(startPos, endPos, Color.White);
						}
					

						//DrawLine
						connectionLine.Draw(worldRotate, viewMatrix, projectionMatrix);
					}
				}
				
			}
		}



		/// <summary>
		/// Draw distal synapse connections for chosen cells
		/// </summary>
		/// <param name="worldTransUp"></param>
		/// <param name="worldRotate"></param>
		/// <param name="col"></param>
		/// <param name="cell"></param>
		private void DrawDistalSynapseConnections(ref Matrix worldTrans, ref Matrix worldRotate, Column col, Cell cell)
		{
			//Draw Connections if existing
			if (cell.IsDataGridSelected || (xnaControl1.showTemporalLearning && cell.isPredicting))
			{
				foreach (Segment seg in cell.segments)
				{
					foreach (Synapse syn in seg.synapses)
					{
						DistalSynapse dSyn = syn as DistalSynapse;
						//Get the two Vectors to draw Line between -> Start Pos
						Vector3 startPos = new Vector3(col.cPos.X, cell.Index, col.cPos.Y);
						//Get InputSource-Pos:
						int x = dSyn.inputSource.column.cPos.X;
						int y = dSyn.inputSource.Index;
						int z = dSyn.inputSource.column.cPos.Y;
						Vector3 endPos = new Vector3(x, y, z);

						if (dSyn.isActive)
						{

							connectionLine.SetUpVertices(startPos, endPos, Color.Black);
						}
						else
						{
							connectionLine.SetUpVertices(startPos, endPos, Color.White);
						}

						//DrawLine
						connectionLine.Draw(worldTrans * worldRotate, viewMatrix, projectionMatrix);
					}
				}
			}
		}


	   

		/// <summary>
		/// 
		/// </summary>
		private void DrawInputSpace(Column col)
		{
			if (col.IsDataGridSelected || (xnaControl1.showInputSpace && col.IsActive))
			{
				Color color;
				float alphaValue = 0.4f;

				int widthStart = col.ColInputSpace.minY;
				int withdStop = col.ColInputSpace.maxY;
				int heightStart = col.ColInputSpace.minX;
				int heightStop = col.ColInputSpace.maxX;

				Matrix worldTrans;
				Matrix worldTransBehindDown = Matrix.CreateTranslation(new Vector3(0, YHTMPLANE-0.01f, 0)) *
					Matrix.CreateTranslation(new Vector3(0, 0, ZHTMPLANE));
				Matrix worldScale = Matrix.CreateScale(new Vector3(0.45f, 0.45f, 0.45f));
				Matrix worldRotate = Matrix.CreateRotationX(pitchHTM) * Matrix.CreateRotationY(yawHTM);

				for (int x = heightStart; x <= heightStop; x++)
				{
					for (int z = widthStart; z <= withdStop; z++)
					{
						Vector3 transVector = new Vector3(x, 0, z);
						worldTrans = Matrix.CreateTranslation(transVector) * worldTransBehindDown;
						Matrix world = worldScale * worldTrans * worldRotate;

						color = Color.Goldenrod;
		
						//Draw input bit
						bit.Draw(world, viewMatrix, projectionMatrix, color, alphaValue);
					}
				}
			}
		}

		/// <summary>
		/// Draws input bitmap. attention: planes is translated also according to constants for better positioning
		/// </summary>
		private void DrawHTMPlane()
		{

			//Get Data from Input Region-> Attention: Draw rhythm happens very often!
			int[,] inputBMRegion = Controler.InputRegion.GetOutPut();

			if (inputBMRegion != null)
			{
				Color color;
				float alphaValue = 0.9f;

				int regionWidth = inputBMRegion.GetLength(0);
				int regionHeight = inputBMRegion.GetLength(1);

				Matrix worldTrans;
				Matrix worldTransBehindDown = Matrix.CreateTranslation(new Vector3(0,YHTMPLANE, 0)) *
					Matrix.CreateTranslation(new Vector3(0,0,ZHTMPLANE));
				Matrix worldScale = Matrix.CreateScale(new Vector3(0.3f, 0.3f, 0.3f));
				Matrix worldRotate = Matrix.CreateRotationX(pitchHTM) * Matrix.CreateRotationY(yawHTM);

				for (int x = 0; x < regionWidth; x++)
				{
					for (int z = 0; z < regionHeight; z++)
					{
						Vector3 transVector = new Vector3(x, 0, z );
						worldTrans = Matrix.CreateTranslation(transVector) * worldTransBehindDown;
						Matrix world = worldScale * worldTrans * worldRotate;

						//Create Color
						if (inputBMRegion[x, z] == 0)
						{
							color = Color.White;
						}
						else
						{
							color = Color.Black;
						}

						//Draw input bit
						bit.Draw(world, viewMatrix, projectionMatrix, color, alphaValue);
					}
				}
			}
		}

		#endregion
	}
}
