using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Controllers
{
    public class BLEController : Controller
    {
        ElectricMeterEntities clsElectric = new ElectricMeterEntities();
        // GET: BLE
        public ActionResult Index()
        {
            BluetoothClient client = new BluetoothClient();
            BluetoothDeviceInfo[] devices = client.DiscoverDevices();
            foreach (BluetoothDeviceInfo device in devices)
            {
                Console.WriteLine($"Found device: {device.DeviceName} [{device.DeviceAddress}]");
                // Optionally filter devices by name or other criteria
            }
            return View();
        }
        
    }
}