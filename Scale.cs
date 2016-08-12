/*
 * Created by SharpDevelop.
 * User: nricciar
 * Date: 10/8/2010
 * Time: 3:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ScaleInterface
{
  using HidLibrary;
  using System.Threading;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  class USBScale
  {
    public bool IsConnected
    {
      get {
        return scale == null ? false : scale.IsConnected;
      }
    }
    public decimal ScaleStatus
    {
      get {
        return inData.Data[1];
      }
    }
    public decimal ScaleWeightUnits
    {
      get {
        return inData.Data[2];
      }
    }
    private HidDevice scale;
    private HidDeviceData inData;

    public HidDevice GetDevice ()
    {
      HidDevice hidDevice;
      // Stamps.com Scale
      hidDevice = HidDevices.Enumerate(0x1446, 0x6A73).FirstOrDefault();
      if (hidDevice != null)
        return hidDevice;

      // Metler Toledo
      hidDevice = HidDevices.Enumerate(0x0eb8).FirstOrDefault();
      if (hidDevice != null)
        return hidDevice;

      return null;
    }
    public bool Connect ()
    {
      // Find a Scale
      HidDevice device = GetDevice();
      if (device != null)
        return Connect(device);
      else
        return false;
    }
    public bool Connect (HidDevice device)
    {
      scale = device;
      int waitTries = 0;
      scale.OpenDevice();

      // sometimes the scale is not ready immedietly after
      // Open() so wait till its ready
      while (!this.IsConnected && waitTries < 10)
      {
        Thread.Sleep(50);
        waitTries++;
      }
      return this.IsConnected;
    }
    public void Disconnect ()
    {
      if (this.IsConnected)
      {
        scale.CloseDevice();
        scale.Dispose();
      }
    }
    public void DebugScaleData ()
    {
      for (int i = 0; i < inData.Data.Length; ++i)
      {
        Console.WriteLine("Byte {0}: {1}", i, inData.Data[i]);
      }
    }
    public void GetWeight (out decimal? weight, out bool? isStable)
    {
      weight = null;
      isStable = false;

      if (this.IsConnected)
      {
        inData = scale.Read(250);
        // Byte 0 == Report ID?
        // Byte 1 == Scale Status (1 == Fault, 2 == Stable @ 0, 3 == In Motion, 4 == Stable, 5 == Under 0, 6 == Over Weight, 7 == Requires Calibration, 8 == Requires Re-Zeroing)
        // Byte 2 == Weight Unit
        // Byte 3 == Data Scaling (decimal placement) - signed byte is power of 10
        // Byte 4 == Weight LSB
        // Byte 5 == Weight MSB

        weight = (Convert.ToDecimal(inData.Data[4]) + 
            Convert.ToDecimal(inData.Data[5]) * 256) *
            Convert.ToDecimal(Math.Pow(10, (sbyte)inData.Data[3]));

        switch (Convert.ToInt16(inData.Data[2]))
        {
          case 3:  // Kilos
            weight = weight * (decimal?)2.2;
            break;
          case 11: // Ounces
            weight = weight * (decimal?)0.0625;
            break;
          case 12: // Pounds
            // already in pounds, do nothing
            break;
        }
        isStable = inData.Data[1] == 0x4;
      }
    }
  }
}
