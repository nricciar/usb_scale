/*
 *  * Created by SharpDevelop.
 *   * User: nricciar
 *    * Date: 10/7/2010
 *     * Time: 11:53 AM
 *      * 
 *       * To change this template use Tools | Options | Coding | Edit Standard Headers.
 *        */
using System;
using System.Threading;
using HIDLibrary;
using ScaleInterface;

namespace ScaleReader
{
  class Program
  {
    public static void Main(string[] args)
    {
      USBScale s = new USBScale();
      s.Connect();
      decimal? pounds;
      bool? isStable;
      s.GetWeight(out pounds, out isStable);
      s.DebugScaleData();
      Console.WriteLine("Weight: {0} LBS", pounds);
      Thread.Sleep(5000);
    }
  }
}
