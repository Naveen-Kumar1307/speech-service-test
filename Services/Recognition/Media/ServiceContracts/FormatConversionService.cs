using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

namespace GlobalEnglish.Media.ServiceContracts
{
    /// <summary>
    /// Converts encoded media from one format to another.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a pair of media formats</item>
    /// <item>converts supplied data from one media format to another</item>
    /// </list>
    /// </remarks>
    [ServiceContract]
    public interface IFormatConversionService
    {
        /// <summary>
        /// Converts the supplied data from one media format to another.
        /// </summary>
        /// <param name="formattedData">formatted data</param>
        /// <param name="fileName">a file name</param>
        /// <returns>transformed / transcoded data</returns>
        byte[] ConvertFormat(byte[] formattedData, string fileName);

    } // IFormatConversionService
}
