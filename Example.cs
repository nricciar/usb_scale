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
using HidLibrary;
using ScaleInterface;

namespace ScaleReader
{
  class Program
  {
    public static void Main(string[] args)
    {
      decimal? weight;
      bool? isStable;

      USBScale s = new USBScale();
      s.Connect();

      if (s.IsConnected)
      {
        s.GetWeight(out weight, out isStable);
        s.DebugScaleData();
        s.Disconnect();
        Console.WriteLine("Weight: {0:0.00} LBS", weight);
      } else {
        Console.WriteLine("No Scale Connected.");
      }
    }
  }
}
