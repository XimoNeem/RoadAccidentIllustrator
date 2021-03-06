// -----------------------------------------------------------------------
// <copyright file="Behavior.cs">
// Original Triangle code by Jonathan Richard Shewchuk, http://www.cs.cmu.edu/~quake/triangle.html
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

using UnityEngine;

namespace TriangleNet
{
    using System;
    using TriangleNet.Geometry;

    /// <summary>
    /// Controls the behavior of the meshing software.
    /// </summary>
    class Behavior
    {
        bool poly = false;
        bool quality = false;
        bool varArea = false;
        bool convex = false;
        bool jettison = false;
        bool boundaryMarkers = true;
        bool noHoles = false;
        bool conformDel = false;

        Func<ITriangle, float, bool> usertest;

        int noBisect = 0;

        float minAngle = 0.0f;
        float maxAngle = 0.0f;
        float maxArea = -1.0f;

        internal bool fixedArea = false;
        internal bool useSegments = true;
        internal bool useRegions = false;
        internal float goodAngle = 0.0f;
        internal float maxGoodAngle = 0.0f;
        internal float offconstant = 0.0f;

        /// <summary>
        /// Creates an instance of the Behavior class.
        /// </summary>
        public Behavior(bool quality = false, float minAngle = 20.0f)
        {
            if (quality)
            {
                this.quality = true;
                this.minAngle = minAngle;

                Update();
            }
        }

        /// <summary>
        /// Update quality options dependencies.
        /// </summary>
        private void Update()
        {
            this.quality = true;

            if (this.minAngle < 0 || this.minAngle > 60)
            {
                this.minAngle = 0;
                this.quality = false;

                Log.Instance.Warning("Invalid quality option (minimum angle).", "Mesh.Behavior");
            }

            if ((this.maxAngle != 0.0) && (this.maxAngle < 60 || this.maxAngle > 180))
            {
                this.maxAngle = 0;
                this.quality = false;

                Log.Instance.Warning("Invalid quality option (maximum angle).", "Mesh.Behavior");
            }

            this.useSegments = this.Poly || this.Quality || this.Convex;
            this.goodAngle = Mathf.Cos(this.MinAngle * Mathf.PI / 180.0f);
            this.maxGoodAngle = Mathf.Cos(this.MaxAngle * Mathf.PI / 180.0f);

            if (this.goodAngle == 1.0f)
            {
                this.offconstant = 0.0f;
            }
            else
            {
                this.offconstant = 0.475f * Mathf.Sqrt((1.0f + this.goodAngle) / (1.0f - this.goodAngle));
            }

            this.goodAngle *= this.goodAngle;
        }

        #region Static properties

        /// <summary>
        /// No exact arithmetic.
        /// </summary>
        public static bool NoExact { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Quality mesh generation.
        /// </summary>
        public bool Quality
        {
            get { return quality; }
            set
            {
                quality = value;
                if (quality)
                {
                    Update();
                }
            }
        }

        /// <summary>
        /// Minimum angle constraint.
        /// </summary>
        public float MinAngle
        {
            get { return minAngle; }
            set { minAngle = value; Update(); }
        }

        /// <summary>
        /// Maximum angle constraint.
        /// </summary>
        public float MaxAngle
        {
            get { return maxAngle; }
            set { maxAngle = value; Update(); }
        }

        /// <summary>
        /// Maximum area constraint.
        /// </summary>
        public float MaxArea
        {
            get { return maxArea; }
            set
            {
                maxArea = value;
                fixedArea = value > 0.0f;
            }
        }

        /// <summary>
        /// Apply a maximum triangle area constraint.
        /// </summary>
        public bool VarArea
        {
            get { return varArea; }
            set { varArea = value; }
        }

        /// <summary>
        /// Input is a Planar Straight Line Graph.
        /// </summary>
        public bool Poly
        {
            get { return poly; }
            set { poly = value; }
        }

        /// <summary>
        /// Apply a user-defined triangle constraint.
        /// </summary>
        public Func<ITriangle, float, bool> UserTest
        {
            get { return usertest; }
            set { usertest = value; }
        }

        /// <summary>
        /// Enclose the convex hull with segments.
        /// </summary>
        public bool Convex
        {
            get { return convex; }
            set { convex = value; }
        }

        /// <summary>
        /// Conforming Delaunay (all triangles are truly Delaunay).
        /// </summary>
        public bool ConformingDelaunay
        {
            get { return conformDel; }
            set { conformDel = value; }
        }

        /// <summary>
        /// Suppresses boundary segment splitting.
        /// </summary>
        /// <remarks>
        /// 0 = split segments
        /// 1 = no new vertices on the boundary
        /// 2 = prevent all segment splitting, including internal boundaries
        /// </remarks>
        public int NoBisect
        {
            get { return noBisect; }
            set
            {
                noBisect = value;
                if (noBisect < 0 || noBisect > 2)
                {
                    noBisect = 0;
                }
            }
        }

        /// <summary>
        /// Compute boundary information.
        /// </summary>
        public bool UseBoundaryMarkers
        {
            get { return boundaryMarkers; }
            set { boundaryMarkers = value; }
        }

        /// <summary>
        /// Ignores holes in polygons.
        /// </summary>
        public bool NoHoles
        {
            get { return noHoles; }
            set { noHoles = value; }
        }

        /// <summary>
        /// Jettison unused vertices from output.
        /// </summary>
        public bool Jettison
        {
            get { return jettison; }
            set { jettison = value; }
        }

        #endregion
    }
}
