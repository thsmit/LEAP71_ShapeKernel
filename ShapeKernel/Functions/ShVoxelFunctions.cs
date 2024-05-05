//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2023 by LEAP 71
// https://leap71.com
//
// Computational Engineering will profoundly change our physical world in the
// years ahead. Thank you for being part of the journey.
//
// We have developed this library to be used widely, for both commercial and
// non-commercial projects alike. Therefore, have released it under a permissive
// open-source license.
// 
// The LEAP 71 ShapeKernel is based on the PicoGK compact computational geometry 
// framework. See https://picogk.org for more information.
//
// LEAP 71 licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, THE SOFTWARE IS
// PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
//
// See the License for the specific language governing permissions and
// limitations under the License.   
//


using System.Numerics;
using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        public partial class Sh
        {
            /// <summary>
            /// Returns a point on the voxelfield that is closest to the specified point.
            /// "Snapp to surface".
            /// </summary>
            public static Vector3 vecGetClosestSurfacePoint(Voxels oVoxels, Vector3 vecPt)
            {
                bool bValid = oVoxels.bClosestPointOnSurface(in vecPt, out Vector3 vecSurface);
                return vecSurface;
            }

            /// <summary>
            /// Returns a point on the voxelfield as a result from raycasting from the specified point into the specidied direction.
            /// "Project to surface".
            /// </summary>
            public static Vector3 vecGetProjectedSurfacePoint(Voxels oVoxels, Vector3 vecPt, Vector3 vecDir)
            {
                bool bValid = oVoxels.bRayCastToSurface(in vecPt, in vecDir, out Vector3 vecSurface);
                return vecSurface;
            }

            /// <summary>
            /// Returns the axis-aligned bounding box of a voxelfield.
            /// </summary>
            public static BBox3 oGetBoundingBox(Voxels oVoxels)
            {
                oVoxels.CalculateProperties(out float fCubicMM, out BBox3 oBox);
                return oBox;
            }

            /// <summary>
            /// Returns the centre of gravity of a voxelfield.
            /// The function creates a slice stack from the voxelfield and
            /// sums up all the "inside" voxel positions in each slice.
            /// The centre of gravity results from the sum being devided by the counted number of voxels.
            /// "Outside" voxels with no matter are ignored.
            /// </summary>
            public static Vector3 vecGetCentreOfGravity(Voxels oVoxels)
            {
                oVoxels.GetVoxelDimensions(out int iXSize, out int iYSize, out int iZSize);
                BBox3 oBBox             = oGetBoundingBox(oVoxels);
                ImageGrayScale oImage   = new ImageGrayScale(iXSize, iYSize);
                Vector3 vecCoG          = new Vector3();
                float fCounter          = 0f;

                for (int iSlice = 0; iSlice < iZSize; iSlice++)
                {
                    try
                    {
                        oVoxels.GetVoxelSlice(in iSlice, ref oImage);

                        for (int i = 0; i < oImage.nHeight; i++)
                        {
                            for (int j = 0; j < oImage.nWidth; j++)
                            {
                                float fValue = oImage.fValue(j, i);
                                if (fValue <= 0)
                                {
                                    float fXRatio = (float)j / (float)oImage.nWidth;
                                    float fYRatio = (float)i / (float)oImage.nHeight;
                                    float fZRatio = (float)iSlice / (float)iZSize;
                                    Vector3 vecPt = oBBox.vecMin +
                                                        fXRatio * oBBox.vecSize().X * Vector3.UnitX +
                                                        fYRatio * oBBox.vecSize().Y * Vector3.UnitY +
                                                        fZRatio * oBBox.vecSize().Z * Vector3.UnitZ;

                                    fCounter    += 1f;
                                    vecCoG      += vecPt;
                                }
                            }
                        }
                    }
                    catch { }
                }
                vecCoG = vecCoG / fCounter;
                return vecCoG;
            }
        }
    }
}