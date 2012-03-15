using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Transmute.Windows;
using System.Windows.Controls.Primitives;
using System.IO;

using xColor = Microsoft.Xna.Framework.Color;
using wColor = System.Windows.Media.Color;
using wColors = System.Windows.Media.Colors;
using Microsoft.Xna.Framework.Input;
using System.Windows.Interop;

namespace Transmute.Wpf
{
    public partial class XnaViewer : UserControl
    {
        #region Fields

        private bool _ScrollBarsVisible = true;
        private bool _ToolbarVisible = true;
        private bool _ShowGrid = true;
        private MouseState previousMouseState = Mouse.GetState();

        #endregion

        #region Properties


        public SpriteFont myFont;
        public SpriteBatch SpriteBatch;
        public Color ClearColor = Color.Black;

        public Texture2D MouseCursor = null;
        public Texture2D WhitePixel = null;
        public Texture2D Grid = null;

        public Vector2 MouseCursorSize = Vector2.One;
        public Color MouseCursorColor = Color.White;
        public Vector2 MouseCursorOffset = Vector2.Zero;

        public Vector2 GridSize = new Vector2(14f);
        public string GridAssetName = ".\\Textures\\TileGrid.png";

        public bool showTemporalLearning=false;
        public bool showSpatialLearning=false;
        public bool showInputSpace = false;
        public bool showCoordinateSystem = false;


        public Color GridColor = new Color(255, 255, 255, 128);

        public int MouseWheelValue = 0;

        public Window ParentWindow = null;

        public float Zoom = 1f;

        public bool ShowGrid
        {
            get
            {
                return _ShowGrid;
            }
            set
            {
                _ShowGrid = value;
            }
        }

        public bool ScrollBarsVisible
        {
            get
            {
                return _ScrollBarsVisible;
            }
            set
            {
                _ScrollBarsVisible = value;
                vScrollBar.Visibility = (_ScrollBarsVisible ? Visibility.Visible : Visibility.Collapsed);
                hScrollBar.Visibility = (_ScrollBarsVisible ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public bool ToolbarVisible
        {
            get
            {
                return _ToolbarVisible;
            }
            set
            {
                _ToolbarVisible = value;
                MainToolbar.Visibility = (_ToolbarVisible ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public ScrollBar VScrollBar
        {
            get
            {
                return vScrollBar;
            }
        }

        public ScrollBar HScrollBar
        {
            get
            {
                return hScrollBar;
            }
        }

        public ToolBar MainToolbar
        {
            get
            {
                return mainToolbar;
            }
        }

        public Vector2 MousePosition
        {
            get
            {
                return graphicsDeviceControl.MousePosition;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsDeviceControl.GraphicsDevice;
            }
        }

        public GraphicsDeviceService GraphicsService
        {
            get
            {
                return graphicsDeviceControl.GraphicsService;
            }
        }

        public ServiceContainer Services
        {
            get
            {
                return graphicsDeviceControl.Services;
            }
        }

        public bool HasFocus
        {
            get
            {
                return graphicsDeviceControl.HasFocus;
            }
        }

        public int GraphicsDeviceMessage
        {
            get
            {
                return graphicsDeviceControl.Message;
            }
        }

        #endregion

        #region Events

        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;
        public event EventHandler<GraphicsDeviceEventArgs> Draw;
        public event EventHandler<HwndMouseEventArgs> LButtonDown;
        public event EventHandler<HwndMouseEventArgs> LButtonUp;
        public event EventHandler<HwndMouseEventArgs> LButtonDoubleClick;
        public event EventHandler<HwndMouseEventArgs> RButtonDown;
        public event EventHandler<HwndMouseEventArgs> RButtonUp;
        public event EventHandler<HwndMouseEventArgs> RButtonDoubleClick;
        public event EventHandler<HwndMouseEventArgs> MButtonDown;
        public event EventHandler<HwndMouseEventArgs> MButtonUp;
        public event EventHandler<HwndMouseEventArgs> MButtonDoubleClick;
        public event EventHandler<HwndMouseEventArgs> X1ButtonDown;
        public event EventHandler<HwndMouseEventArgs> X1ButtonUp;
        public event EventHandler<HwndMouseEventArgs> X1ButtonDoubleClick;
        public event EventHandler<HwndMouseEventArgs> X2ButtonDown;
        public event EventHandler<HwndMouseEventArgs> X2ButtonUp;
        public event EventHandler<HwndMouseEventArgs> X2ButtonDoubleClick;
        public event EventHandler<HwndMouseEventArgs> XnaMouseMove;
        public event EventHandler<HwndMouseEventArgs> XnaMouseEnter;
        public event EventHandler<HwndMouseEventArgs> XnaMouseLeave;
        public event EventHandler<HwndUpdateEventArgs> Update;
        public event EventHandler OnDrawMouseCursor;
        public event EventHandler<ScrollBarEventArgs> OnScroll;

        #endregion

        #region Constructor

        public XnaViewer()
        {
            InitializeComponent();
            CreateDefaultContextMenu();

            ShowGrid = false;
            MouseEnter += XnaViewer_MouseEnter;
            PreviewMouseMove += XnaViewer_PreviewMouseMove;
            MouseMove += XnaViewer_PreviewMouseMove;
        }

        #endregion

        #region GraphicsDeviceControl Event Methods

        private void graphicsDeviceControl_LoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            LoadLocalContent(e.GraphicsDevice);
            if (LoadContent != null) LoadContent(sender, e);
        }

        private void graphicsDeviceControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            try
            {
                if (Draw != null) Draw(sender, e);
                DrawGrid();
                DrawMouseCursor();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void graphicsDeviceControl_MouseMove(object sender, HwndMouseEventArgs e)
        {
            if (XnaMouseMove != null) XnaMouseMove(sender, e);
        }

        private void graphicsDeviceControl_HwndLButtonDown(object sender, HwndMouseEventArgs e)
        {
            if (LButtonDown != null) LButtonDown(sender, e);
        }

        private void graphicsDeviceControl_HwndLButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (LButtonUp != null) LButtonUp(sender, e);
        }

        private void graphicsDeviceControl_HwndLButtonDblClick(object sender, HwndMouseEventArgs e)
        {
            if (LButtonDoubleClick != null) LButtonDoubleClick(sender, e);
        }

        private void graphicsDeviceControl_HwndMButtonDblClick(object sender, HwndMouseEventArgs e)
        {
            if (MButtonDoubleClick != null) MButtonDoubleClick(sender, e);
        }

        private void graphicsDeviceControl_HwndMButtonDown(object sender, HwndMouseEventArgs e)
        {
            if (MButtonDown != null) MButtonDown(sender, e);
        }

        private void graphicsDeviceControl_HwndMButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (MButtonUp != null) MButtonUp(sender, e);
        }

        private void graphicsDeviceControl_HwndMouseEnter(object sender, HwndMouseEventArgs e)
        {
            graphicsDeviceControl.SetFocus();
            if (XnaMouseEnter != null) XnaMouseEnter(sender, e);
        }

        private void graphicsDeviceControl_HwndMouseLeave(object sender, HwndMouseEventArgs e)
        {
            if (XnaMouseLeave != null) XnaMouseLeave(sender, e);
        }

        private void graphicsDeviceControl_HwndRButtonDblClick(object sender, HwndMouseEventArgs e)
        {
            if (RButtonDoubleClick != null) RButtonDoubleClick(sender, e);
        }

        private void graphicsDeviceControl_HwndRButtonDown(object sender, HwndMouseEventArgs e)
        {
            OpenContextMenu();
            if (RButtonDown != null) RButtonDown(sender, e);
        }

        private void graphicsDeviceControl_HwndRButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (RButtonUp != null) RButtonUp(sender, e);
        }

        private void graphicsDeviceControl_HwndX1ButtonDblClick(object sender, HwndMouseEventArgs e)
        {
            if (X1ButtonDoubleClick != null) X1ButtonDoubleClick(sender, e);
        }

        private void graphicsDeviceControl_HwndX1ButtonDown(object sender, HwndMouseEventArgs e)
        {
            if (X1ButtonDown != null) X1ButtonDown(sender, e);
        }

        private void graphicsDeviceControl_HwndX1ButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (X1ButtonUp != null) X1ButtonUp(sender, e);
        }

        private void graphicsDeviceControl_HwndX2ButtonDblClick(object sender, HwndMouseEventArgs e)
        {
            if (X2ButtonDoubleClick != null) X2ButtonDoubleClick(sender, e);
        }

        private void graphicsDeviceControl_HwndX2ButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (X2ButtonUp != null) X2ButtonUp(sender, e);
        }

        private void graphicsDeviceControl_HwndX2ButtonDown(object sender, HwndMouseEventArgs e)
        {
            if (X2ButtonDown != null) X2ButtonDown(sender, e);
        }

        private void graphicsDeviceControl_UpdateXna(object sender, HwndUpdateEventArgs e)
        {
            UpdateMouse();
            if (Update != null) Update(sender, e);
        }

        #endregion

        #region Methods

        #region Context Menu Methods

        private void OpenContextMenu(FrameworkElement element)
        {
            if (element.ContextMenu != null)
            {
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.IsOpen = true;
            }
        }

        public void OpenContextMenu()
        {
            OpenContextMenu(this);
        }

        public void CreateDefaultContextMenu()
        {
            ContextMenu menu = new ContextMenu();

          
            var ShowTempLearning = CreateToggleButtonMenuItem(showTemporalLearning, "Show temporal learning");
            ShowTempLearning.Click += ShowTemporalLearning;
            var button = (ShowTempLearning.Icon as ToggleButton);
            button.Checked += ShowTemporalLearning;
            button.Unchecked += ShowTemporalLearning;
            menu.Items.Add(ShowTempLearning);

            var ShowSpatLearning = CreateToggleButtonMenuItem(showSpatialLearning, "Show spatial learning");
            ShowSpatLearning.Click += ShowSpatialLearning;
            var button2 = (ShowSpatLearning.Icon as ToggleButton);
            button2.Checked += ShowSpatialLearning;
            button2.Unchecked += ShowSpatialLearning;
            menu.Items.Add(ShowSpatLearning);

            var ShowInputSpace = CreateToggleButtonMenuItem(showInputSpace, "Show Input Space");
            ShowInputSpace.Click += BlendInputSpace;
            var button4 = (ShowInputSpace.Icon as ToggleButton);
            button4.Checked += BlendInputSpace;
            button4.Unchecked += BlendInputSpace;
            menu.Items.Add(ShowInputSpace);

            var ShowCooSys = CreateToggleButtonMenuItem(showCoordinateSystem, "Hide/Show Coordinate System");
            ShowCooSys.Click += ShowCoordinateSystem_Click;
            var button3 = (ShowCooSys.Icon as ToggleButton);
            button3.Checked += ShowCoordinateSystem_Click;
            button3.Unchecked += ShowCoordinateSystem_Click;
            menu.Items.Add(ShowCooSys);

            ContextMenu = menu;
        }

        void ShowCoordinateSystem_Click(object sender, RoutedEventArgs e)
        {
            bool value = false;
            ToggleButton button = null;

            if (sender is MenuItem)
            {
                value = (bool)((sender as MenuItem).Tag);
                button = ((sender as MenuItem).Icon as ToggleButton);
                button.IsChecked = !value;
                (sender as MenuItem).Tag = !value;
                showCoordinateSystem = !value;
            }
            if (sender is ToggleButton)
            {
                button = (sender as ToggleButton);
                showCoordinateSystem = (button.IsChecked == true ? true : false);
            }
        }

        void scrollBarsVisible_Click(object sender, RoutedEventArgs e)
        {
            bool value = false;
            ToggleButton button = null;
            if (sender is MenuItem)
            {
                value = (bool)((sender as MenuItem).Tag);
                button = ((sender as MenuItem).Icon as ToggleButton);
                button.IsChecked = !value;
                (sender as MenuItem).Tag = !value;
            }
            if (sender is ToggleButton)
            {
                value = (bool)((sender as ToggleButton).Tag);
                button = (sender as ToggleButton);
                ScrollBarsVisible = (button.IsChecked == true ? true : false);
                value = ScrollBarsVisible;
                (sender as ToggleButton).Tag = value;
            }
        }


        void ShowTemporalLearning(object sender, RoutedEventArgs e)
        {
            bool value = false;
            ToggleButton button = null;

            if (sender is MenuItem)
            {
                value = (bool)((sender as MenuItem).Tag);
                button = ((sender as MenuItem).Icon as ToggleButton);
                button.IsChecked = !value;
                (sender as MenuItem).Tag = !value;
                showTemporalLearning = !value;
            }
            if (sender is ToggleButton)
            {
                button = (sender as ToggleButton);
                showTemporalLearning = (button.IsChecked == true ? true : false);
            }
        }


        void BlendInputSpace(object sender, RoutedEventArgs e)
        {
            bool value = false;
            ToggleButton button = null;

            if (sender is MenuItem)
            {
                value = (bool)((sender as MenuItem).Tag);
                button = ((sender as MenuItem).Icon as ToggleButton);
                button.IsChecked = !value;
                (sender as MenuItem).Tag = !value;
                showInputSpace = !value;
            }
            if (sender is ToggleButton)
            {
                button = (sender as ToggleButton);
                showInputSpace = (button.IsChecked == true ? true : false);
            }
        }



        void ShowSpatialLearning(object sender, RoutedEventArgs e)
        {
            bool value = false;
            ToggleButton button = null;

            if (sender is MenuItem)
            {
                value = (bool)((sender as MenuItem).Tag);
                button = ((sender as MenuItem).Icon as ToggleButton);
                button.IsChecked = !value;
                (sender as MenuItem).Tag = !value;
                showSpatialLearning = !value;
            }
            if (sender is ToggleButton)
            {
                button = (sender as ToggleButton);
                showSpatialLearning = (button.IsChecked == true ? true : false);
            }
        }

        private MenuItem CreateColorPickerMenuItem(wColor color, string text)
        {
            var menuItem = new MenuItem();
            DockPanel panel = new DockPanel();
            Canvas canvas = new Canvas
            {
                Width = 16,
                Height = 16,
                Background = new System.Windows.Media.SolidColorBrush(color)
            };
            TextBlock txtBlock = new TextBlock()
            {
                Text = text
            };
            panel.Children.Add(txtBlock);
            DockPanel.SetDock(canvas, Dock.Left);
            menuItem.Header = panel;
            menuItem.Tag = color;
            menuItem.Icon = canvas;
            return menuItem;
        }

        private MenuItem CreateToggleButtonMenuItem(bool value, string text)
        {
            var menuItem = new MenuItem();
            ToggleButton button = new ToggleButton()
            {
                IsChecked = value,
                Width = 16,
                Height = 16,
                Tag = value
            };
            menuItem.Header = text;
            menuItem.Icon = button;
            menuItem.Tag = value;
            return menuItem;
        }

        #endregion

        #region Utility Methods

        public static Texture2D LoadTexture2D(GraphicsDevice graphicsDevice, string path)
        {
            return Texture2D.FromStream(graphicsDevice, new StreamReader(path).BaseStream);
        }

        private void LoadLocalContent(GraphicsDevice graphicsDevice)
        {

            SpriteBatch = new SpriteBatch(graphicsDevice);

            LoadLocalFont();

            WhitePixel = new Texture2D(graphicsDevice, 1, 1);
            Color[] pixels = new Color[1];
            pixels[0] = Color.White;
            WhitePixel.SetData<Color>(pixels);

            if (File.Exists(GridAssetName))
            {
                Grid = LoadTexture2D(graphicsDevice, GridAssetName);
            }
        }

        private void LoadLocalFont()
        {
            ContentManager content;
            content = new ContentManager(Services);
            try
            {
                myFont = content.Load<SpriteFont>("myFont");
            }
            catch (ContentLoadException le)
            {
                Debug.WriteLine("Font could not be loaded");
                Debug.WriteLine(le);
            }
        }

        private void DrawMouseCursor()
        {
            if (OnDrawMouseCursor != null)
            {
                OnDrawMouseCursor(this, EventArgs.Empty);
            }
            else
            {
                if (MouseCursor != null)
                {
                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                    Rectangle destRect = new Rectangle((int)(graphicsDeviceControl.MousePosition.X + MouseCursorOffset.X), (int)(graphicsDeviceControl.MousePosition.Y + MouseCursorOffset.Y), (int)(MouseCursorSize.X * Zoom), (int)(MouseCursorSize.Y * Zoom));
                    SpriteBatch.Draw(MouseCursor, destRect, MouseCursorColor);
                    SpriteBatch.End();
                }
            }
        }

        private void DrawGrid()
        {
            if (ShowGrid && Grid != null)
            {
                float width = (GridSize.X * Zoom);
                float height = (GridSize.Y * Zoom);

                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null);
                for (float y = 0; y < ActualHeight; y += height)
                {
                    for (float x = 0; x < ActualWidth; x += width)
                    {
                        SpriteBatch.Draw(Grid, new Rectangle((int)x, (int)y, (int)width, (int)height), GridColor);
                    }
                }
                SpriteBatch.End();
            }
        }

        public void SetMouseCursor(Texture2D texture, Vector2 size, Color color, Vector2 cursorOffset)
        {
            MouseCursor = texture;
            MouseCursorSize = size;
            MouseCursorColor = color;
            MouseCursorOffset = cursorOffset;
        }

        public void SetMouseCursor(Texture2D texture, Color color)
        {
            SetMouseCursor(texture, new Vector2(texture.Width, texture.Height), color, Vector2.Zero);
        }

        public void SetMouseCursor(Texture2D texture)
        {
            SetMouseCursor(texture, new Vector2(texture.Width, texture.Height), Color.White, Vector2.Zero);
        }

        public void SetMouseCursor(Texture2D texture, Vector2 size)
        {
            SetMouseCursor(texture, size, Color.White, Vector2.Zero);
        }

        public void SetMouseCursor(Texture2D texture, Vector2 size, Vector2 cursorOffset)
        {
            SetMouseCursor(texture, size, Color.White, cursorOffset);
        }

        public void MoveContextMenuToToolbar()
        {
            MenuItem menu = new MenuItem();
            menu.Header = "Options";

            var menuItem1 = new MenuItem();
            var ShowTempLearning = CreateToggleButtonMenuItem(showTemporalLearning, "Show temporal learning");
            ShowTempLearning.Click += ShowTemporalLearning;
            var button = (ShowTempLearning.Icon as ToggleButton);
            button.Checked += ShowTemporalLearning;
            button.Unchecked += ShowTemporalLearning;
            menuItem1.Header = ShowTempLearning;
            menu.Items.Add(menuItem1);

            var menuItem2 = new MenuItem();
            var ShowSpatLearning = CreateToggleButtonMenuItem(showSpatialLearning, "Show spatial learning");
            ShowSpatLearning.Click += ShowSpatialLearning;
            var button2 = (ShowSpatLearning.Icon as ToggleButton);
            button.Checked += ShowTemporalLearning;
            button.Unchecked += ShowTemporalLearning;
            menu.Items.Add(menuItem2);

            MainToolbar.Items.Add(menu);

            ContextMenu = null;
        }

        public void SetFocus()
        {
            graphicsDeviceControl.SetFocus();
        }

        public void UpdateMouse()
        {
            //if (!HasFocus) return;
            if (ParentWindow == null) return;
            try { 
                Mouse.WindowHandle = (PresentationSource.FromVisual(ParentWindow) as HwndSource).Handle;
                MouseState mouse = Mouse.GetState();
                var delta = mouse.ScrollWheelValue - previousMouseState.ScrollWheelValue;
                MouseWheelValue = delta;
                previousMouseState = mouse;
            }
            catch(Exception ex){

                Debug.WriteLine(ex);
            }

        }

        #endregion

        #region Events

        void XnaViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            graphicsDeviceControl.SetFocus();
        }

        void XnaViewer_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            graphicsDeviceControl.SetFocus();
        }

        private void vScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OnScroll != null) OnScroll(this, new ScrollBarEventArgs(hScrollBar, vScrollBar));
        }

        private void hScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OnScroll != null) OnScroll(this, new ScrollBarEventArgs(hScrollBar, vScrollBar));
        }

        #endregion
        
        #endregion
        
    }
}
