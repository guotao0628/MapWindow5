﻿using System;
using MapWinGIS;
using MW5.Api.Concrete;
using MW5.Api.Helpers;
using MW5.Api.Interfaces;

namespace MW5.Api.Static
{
    public static class GeoProcessing
    {
        private static Utils _utils = new Utils();

        public static bool PointInPolygon(IGeometry geometry, ICoordinate testPoint)
        {
            return _utils.PointInPolygon(geometry.GetInternal(), testPoint.GetInternal());
        }

        public static bool GridReplace(Grid grid, object oldValue, object newValue)
        {
            return _utils.GridReplace(grid, oldValue, newValue);
        }

        public static bool GridInterpolateNoData(Grid gird)
        {
            return _utils.GridInterpolateNoData(gird);
        }

        public static bool RemoveColinearPoints(IFeatureSet feature, double linearTolerance)
        {
            return _utils.RemoveColinearPoints(feature.GetInternal(), linearTolerance);
        }

        public static GridSource GridMerge(GridSource[] grids, string mergeFilename, bool inRam = true,
                                            GridFormat format = GridFormat.UseExtension)
        {
            var grid = _utils.GridMerge(grids, mergeFilename, inRam, (GridFileType) format);
            return grid != null ? new GridSource(grid) : null;
        }

        public static IGeometry ShapeMerge(IFeatureSet featureSet, int indexOne, int indexTwo)
        {
            var shape = _utils.ShapeMerge(featureSet.GetInternal(), indexOne, indexTwo);
            return shape != null ? new Geometry(shape) : null;
        }

        public static Image GridToImage(GridSource grid, GridColorRamp scheme)
        {
            return _utils.GridToImage(grid.GetInternal(), scheme.GetInternal());
        }

        public static IFeatureSet GridToShapefile(GridSource grid, GridSource connectionGrid = null)
        {
            var sf = _utils.GridToShapefile(grid.GetInternal(), connectionGrid.GetInternal());
            return sf != null ? new FeatureSet(sf) : null;
        }

        public static GridSource GridToGrid(GridSource grid, DataType outDataType)
        {
            var result = _utils.GridToGrid(grid.GetInternal(), (GridDataType)outDataType);
            return result != null ? new GridSource(result) : null;
        }

        public static IFeatureSet ShapeToShapeZ(IFeatureSet shapefile, GridSource grid)
        {
            var sf = _utils.ShapeToShapeZ(shapefile.GetInternal(), grid.GetInternal());
            return sf != null ? new FeatureSet(sf) : null;
        }

        public static Grid ShapefileToGrid(IFeatureSet shapefile, bool useShapefileBounds = true, 
            GridSourceHeader gridHeader = null, double cellsize = 30, bool useShapeNumber = true, short singleValue = 1)
        {
            return _utils.ShapefileToGrid(shapefile.GetInternal(), useShapefileBounds, 
                        gridHeader.GetInternal(), cellsize, useShapeNumber, singleValue);
        }

        public static bool GenerateHillShade(string gridFilename, string shadeFilename, 
            float z = 1, float scale = 1, float az = 315, float alt = 45)
        {
            return _utils.GenerateHillShade(gridFilename, shadeFilename, z, scale, az, alt);
        }

        public static bool GenerateContour(string pszSrcFilename, string pszDstFilename, double dfInterval, double dfNoData = 0,
            bool is3D = false, object dblFlArray = null)
        {
            // TODO: check last parameter
            return _utils.GenerateContour(pszSrcFilename, pszDstFilename, dfInterval, dfNoData, is3D, dblFlArray);
        }

        public static bool TranslateRaster(string srcFilename, string dstFilename, string bstrOptions)
        {
            return _utils.TranslateRaster(srcFilename, dstFilename, bstrOptions);
        }

        public static bool MergeImages(string[] inputNames, string outputName)
        {
            return _utils.MergeImages(inputNames, outputName);
        }

        public static IFeatureSet ReprojectShapefile(IFeatureSet sf, ISpatialReference source, ISpatialReference target)
        {
            var result = _utils.ReprojectShapefile(sf.GetInternal(), source.GetInternal(), target.GetInternal());
            return result != null ? new FeatureSet(result) : null;
        }

        public static bool ClipGridWithPolygon(string inputGridfile, IGeometry poly, string resultGridfile, bool keepExtents)
        {
            return _utils.ClipGridWithPolygon(inputGridfile, poly.GetInternal(), resultGridfile, keepExtents);
        }

        public static bool ClipGridWithPolygon2(GridSource inputGrid, IGeometry poly, string resultGridfile, bool keepExtents)
        {
            return _utils.ClipGridWithPolygon2(inputGrid.GetInternal(), poly.GetInternal(), resultGridfile, keepExtents);
        }

        public static bool GridStatisticsToShapefile(GridSource grid, IFeatureSet sf, bool selectedOnly, bool overwriteFields)
        {
            return _utils.GridStatisticsToShapefile(grid.GetInternal(), sf.GetInternal(), selectedOnly, overwriteFields);
        }

        public static bool Polygonize(string pszSrcFilename, string pszDstFilename, int iSrcBand = 1, bool noMask = false,
            string pszMaskFilename = "0", string pszOgrFormat = "GML", string pszDstLayerName = "out",
            string pszPixValFieldName = "DN")
        {
            return _utils.Polygonize(pszSrcFilename, pszDstFilename, iSrcBand, noMask, pszMaskFilename, pszOgrFormat, pszDstLayerName, pszPixValFieldName);
        }

        public static bool ConvertDistance(UnitsOfMeasure sourceUnit, UnitsOfMeasure targetUnit, ref double value)
        {
            return _utils.ConvertDistance((tkUnitsOfMeasure)sourceUnit, (tkUnitsOfMeasure)targetUnit, ref value);
        }

        public static double GeodesicDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return _utils.GeodesicDistance(lat1, lng1, lat2, lng2);
        }

        public static bool MaskRaster(string filename, byte newPerBandValue)
        {
            return _utils.MaskRaster(filename, newPerBandValue);
        }

        public static bool GridStatisticsForPolygon(GridSource grid, GridSourceHeader header, IEnvelope gridExtents, 
                IGeometry polygon, double nodataValue, ref double meanValue, ref double minValue, ref double maxValue)
        {
            return _utils.GridStatisticsForPolygon(grid.GetInternal(), header.GetInternal(), gridExtents.GetInternal(),
             polygon.GetInternal(), nodataValue, ref meanValue, ref minValue, ref maxValue);
        }

        public static bool CopyNodataValues(string sourceFilename, string destFilename)
        {
            return _utils.CopyNodataValues(sourceFilename, destFilename);
        }

        public static IImageSource GridToImage2(GridSource grid, GridColorRamp scheme, GridProxyFormat imageFormat, 
                bool inRam)
        {
            var img = _utils.GridToImage2(grid.GetInternal(), scheme.GetInternal(), (tkGridProxyFormat)imageFormat, inRam, null);
            return BitmapSource.Wrap(img);
        }

        public static ISpatialReference TileProjectionToGeoProjection(TileProjection projection)
        {
            var gp = _utils.TileProjectionToGeoProjection((tkTileProjection)projection);
            return gp != null ? new SpatialReference(gp) : null;
        }

        public static bool CalculateRaster(string[] filenames, string expression, string outputFilename, 
            string gdalOutputFormat, float nodataValue, out string errorMsg)
        {
            return _utils.CalculateRaster(filenames, expression, outputFilename, gdalOutputFormat, 
                nodataValue, null, out errorMsg);
        }

        public static bool ReclassifyRaster(string filename, int bandIndex, string outputName, Array lowerBounds, Array upperBounds,
            Array newValues, string gdalOutputFormat)
        {
            return _utils.ReclassifyRaster(filename, bandIndex, outputName, lowerBounds, upperBounds, newValues, gdalOutputFormat, null);
        }

        public static bool IsTiffGrid(string filename)
        {
            return _utils.IsTiffGrid(filename);
        }

        public static string LastError()
        {
            return _utils.ErrorMsg[_utils.LastErrorCode];
        }

        public static string Key
        {
            get { return _utils.Key; }
            set { _utils.Key = value; }
        }

        #region Not implemented

        //public static IGeometry ClipPolygon(PolygonOperation op, IGeometry subjectPolygon, IGeometry clipPolygon)
        //{
        //    return _utils.ClipPolygon(op, subjectPolygon, clipPolygon);
        //}

        //public static Shapefile TinToShapefile(Tin Tin, ShpfileType Type, ICallback cBack)
        //{
        //    return _utils.TinToShapefile(Tin, Type, cBack);
        //}

        //public static IPictureDisp hBitmapToPicture(int hBitmap)
        //{
        //    return _utils.hBitmapToPicture(hBitmap);
        //}

        //public static Shapefile OGRLayerToShapefile(string Filename, ShpfileType ShpType = ShpfileType.SHP_NULLSHAPE, ICallback cBack = null)
        //{
        //    return _utils.OGRLayerToShapefile(Filename, ShpType, cBack);
        //}

        //public static uint ColorByName(tkMapColor Name)
        //{
        //    return _utils.ColorByName(Name);
        //}

        //public static object CreateInstance(tkInterface interfaceId)
        //{
        //    return _utils.CreateInstance(interfaceId);
        //}

        //public static string ErrorMsgFromObject(object comClass)
        //{
        //    return _utils.ErrorMsgFromObject(comClass);
        //}

        #endregion
    }
}