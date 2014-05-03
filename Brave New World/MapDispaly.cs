﻿using System;

using System.Collections.Generic;

using System.ComponentModel;

using System.Data;

using System.Drawing;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Windows;
using System.Windows.Forms;

using VectorLandMesh.Land;
namespace ClassProject
{
    public partial class frmMapDisplay : Form
    {
        //Used in Graphics Liabary
        private Graphics drawing;
        #region User Interactive Properties
            /// <summary>
            /// The Default number of levels
            /// </summary>
            public static int DefaultLevels{ get{return 32;} }

            /// <summary>
            /// Color list each level of the hight map.
            /// </summary>
            public static List<Color> ColorLevelList = new List<Color>();

            /// <summary>
            /// odds map will be generated by settings.
            /// </summary>
            public static int[] OddsMap = { 0, 0, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4 };

            /// <summary>
            /// Number of points on each Contour | Number of levels on a map.
            /// </summary>
            public static int Detail = 16, Levels = DefaultLevels;

            /// <summary>
            /// test for map data. will be set in to class List
            /// </summary>

            public static List<System.Windows.Point> VectorLengths = new List<System.Windows.Point>() { new System.Windows.Point(10, 20), new System.Windows.Point(5, 20) };

            /// <summary>
            /// Value of the seed if it is used
            /// </summary>
            public static int SeedValue;

            /// <summary>
            /// If The SeedValue is being used to seed the Random Object
            /// </summary>
            public static bool useSeed = false;
        #endregion
            /// <summary>
            /// The Personalization Form used to customize the Generation.
            /// </summary>
            private frmPersonalization frmPersonalize;
        public frmMapDisplay()
        {
            InitializeComponent();
            drawing = picboxDrawing.CreateGraphics();
        }
        /// <summary>
        /// 
        /// </summary>
        private void generateMap()
        {
            #region Terrain Generation Variables
            //number of current meshes on each level.
            int meshOnLevel = 0;

            List<LandMesh> mapMeshData = new List<LandMesh>();
            //used later in loop on [Line 46]
            LandMesh land;
            #endregion

            #region Progress Bar variables
            int numberOfContours = 0;
            #endregion

            #region Point Type Conversion Variables
            //list of Points used to draw the Map at Each Level.
            List<List<System.Drawing.Point>> listOfDrawingPoints;
            List<System.Drawing.Point> drawingPoints;
            List<List<List<System.Drawing.Point>>> drawingMapData = new List<List<List<System.Drawing.Point>>>();
            #endregion

            #region Drawing variables
            //ofsets for each point to ensure points are drawn corectly. "Refer to Notes"[AKA Set up notes and ask me in class about this it is kind of complicated.]
            List<int> offsets = new List<int>();
            // "Y - offset refer to above note"[AKA Set up notes and ask me in class about this it is kind of complicated.]
            int y2 = 0;
            #endregion

            #region Setup Color Level List
            //add a color the color list base on a gray scale.
            for (int i = Levels; i > 0; i--)
            {
                ColorLevelList.Add(Color.FromArgb(i * 255 / Levels, i * 255 / Levels, i * 255 / Levels));
            }
            #endregion
            #region Change Form Display
                stripLblStatus.Text = "Generating Terrain";
                stripProgressbar.Visible = false;
            #endregion

            #region Map Initialization
            //Clear drawing area.
            drawing.Clear(Color.Blue);

            //Initialize the map
            if (useSeed)
                Map.InitializeMap(Detail, new float[] { drawing.VisibleClipBounds.X, drawing.VisibleClipBounds.Y, drawing.VisibleClipBounds.Width, drawing.VisibleClipBounds.Height }, SeedValue);
            else
                Map.InitializeMap(Detail, new float[] { drawing.VisibleClipBounds.X, drawing.VisibleClipBounds.Y, drawing.VisibleClipBounds.Width, drawing.VisibleClipBounds.Height });
            #endregion

            #region Terrain Generation          
            //grabs a first randum number from an odds map... but not zero.
            while (meshOnLevel == 0)
            {
                meshOnLevel = OddsMap[Map.MapSeed.Next(0, OddsMap.Length - 1)];
            }

            for (int x = Levels; x > 0; x--)
            {
                //adds new Contour for each existing mesh.                
                foreach (LandMesh mesh in mapMeshData)
                {
                    mesh.addNewContour();
                }

                //add new land mesh(s)
                for (int y = meshOnLevel; y > 0; y--)
                {
                    land = new LandMesh(VectorLengths, 10D, x);
                    mapMeshData.Add(land);
                }

                //grabs a randum number from an odds map.
                meshOnLevel = OddsMap[Map.MapSeed.Next(0, OddsMap.Length - 1)];
            }
            
            #endregion

            #region Change Form Display
            stripLblStatus.Text = "Drawing Terrain";
            stripProgressbar.Visible = true;
            foreach (LandMesh mesh in mapMeshData)
            {
                numberOfContours+= mesh.NumberOfContours;
            }
            stripProgressbar.Maximum = numberOfContours;
            #endregion

            /*
             * Points in Visual C# Forms are System.Drawing.Point (X and Y are int values)
             * Points in LandMesh Class are System.Windows.Point (X and Y are decimal values)
             * So they need to be converted.
             */
            #region Point Type Conversion
            foreach (LandMesh mesh in mapMeshData)
            {
                // resets the "List Of Drawing Points"
                listOfDrawingPoints = new List<List<System.Drawing.Point>>();
                foreach (List<System.Windows.Point> list in mesh.RawPoints)
                {
                    // resets the "Drawing Points"
                    drawingPoints = new List<System.Drawing.Point>();
                    foreach (System.Windows.Point point in list)
                    {
                        //converts the X and Y values of System.Windows.Point (Decimal) to (int) and set them to a new System.Drawing.Point Object
                        drawingPoints.Add(new System.Drawing.Point((int)point.X, (int)point.Y));
                    }
                    // Add the new "Drawing Points" to the "List Of Drawing Points"
                    listOfDrawingPoints.Add(drawingPoints);
                }
                //Add the "List Of Drawing Points" to the "Drawing Map Data"
                drawingMapData.Add(listOfDrawingPoints);
            }
            #endregion

            #region Drawing
            for (int y = Levels; y > 0; y--)
            {

                for (int x = 0; x < mapMeshData.Count; x++)
                {
                    if (y == Levels)
                    {
                        offsets.Add(y - mapMeshData[x].NumberOfLevel);
                    }
                    y2 = y - offsets[x];
                    if (y2 > 0)
                    {
                        System.Drawing.Point[] myPoints = drawingMapData[x][y2 - 1].ToArray<System.Drawing.Point>();
                        drawing.FillClosedCurve(new SolidBrush(ColorLevelList[y - 1]), myPoints);// how to graph the shape using the array of points
                        stripProgressbar.PerformStep();
                    }
                }
            }
            #endregion

            #region Change Form Display
            stripLblStatus.Text = "Finished";
            stripProgressbar.Value=0;
            stripProgressbar.Visible = false;
            #endregion
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generateMap();
        }

        private void personalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmPersonalize == null || frmPersonalize.IsDisposed)
            {
                frmPersonalize = new frmPersonalization();
            }

            frmPersonalize.Show();
        }
    }
}
