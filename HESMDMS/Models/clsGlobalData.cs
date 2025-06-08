using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class clsGlobalData
    {
        public static string HeaderRequest = "";


        public static string myProjName = "";
        public static string myProjTitle = "";
        public static string myVersion = "";

        //public string strRawDataFileName = "";
        //public string strDataFileName = "";
        //public string strReportFileName = "";

        public static string myUserName = "";

        public static string myAppPath = "";

        public static string myReportPath = "";

        public static string strLogFileName = "JioAPI_Log.txt";
        public static string strLoc_LogFile = AppDomain.CurrentDomain.BaseDirectory;

        public static string strLoc_StandardPressure = "";

        public static string strExceptionFileName = "SmartGasMeter_Exception.txt";
        public static string strLoc_ExceptionFile = AppDomain.CurrentDomain.BaseDirectory;

        public static string gblTTBBPrsData = "";
        public static string gblTTBBTemData = "";
        public static string gblTTBBDewData = "";

        public static string gblTTDDPrsData1 = "";
        public static string gblTTDDTemData1 = "";
        public static string gblTTDDDewData1 = "";

        public static string gblTTDDPrsData2 = "";
        public static string gblTTDDTemData2 = "";
        public static string gblTTDDDewData2 = "";

        public string gblSenHead = "";
        public string gblFileHead_Data = "";
        public string gblFileHead_MinMax = "";
        public string gblGridHead = "";

        public static string gblSondeID_DB = "";

        public string[] gblArrSenMast_DefaSenName = new string[7];      ///6 + 1 (dew point)
        public string[] gblArrSenMast_SenName = new string[7];
        public string[] gblArrSenMast_Unit = new string[7];
        public string[] gblArrSenMast_Unit_Mod = new string[7];
        public double[] gblArrSenMast_Reso = new double[7];
        public double[] gblArrSenMast_Min = new double[7];
        public double[] gblArrSenMast_Max = new double[7];

        public int gblFrame_TotalSensor = 0;
        public int gblFrame_RemainPara = 0;
        public int gblFrame_TotalPara = 25;
        public int gblFrame_CheckSumPos = 25;

        public bool flgWebConn = false;

        clsExceptionDataRoutine clsEDR = new clsExceptionDataRoutine();

        public string strModuleName = "Global Data Routines";
        public string strFunctionName = "";
        public string strExceptionMessage = "";

        //public bool flgReport = false;
        //public static bool flgReport = false;
        public static bool flgRepo_Tephigram_TTBB = false;
        public static bool flgRepo_Tephigram_TTDD1 = false;
        public static bool flgRepo_Tephigram_TTDD2 = false;

        public static bool flgRepo_Hodograph = false;

        public static bool flgSondeData = false;
        public static bool flgSondeData_new = false;
        public static bool flgCreateUser = false;
        public static bool flgUserAuthentication = false;
        public static bool flgReport = false;
        public static bool flgBUFR = false;

        public static bool flgCalibration = false;

        public static bool flgFreqTunning = false;

        public static bool flgDataDump = false;

        public static string gblExit = "";

        //public bool funSetSensorPara()
        //{
        //    strFunctionName = "Set Sensor Paramenters";

        //    lock (this)
        //    {
        //        try
        //        {
        //            //Application.DoEvents();

        //            int i;

        //            i = 0;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "AIR TEMPERATURE";
        //            gblArrSenMast_Unit[i] = "(°C)";
        //            gblArrSenMast_Unit_Mod[i] = "(°C)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0.01");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("-100");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("70");

        //            i = 1;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "HUMIDITY";
        //            gblArrSenMast_Unit[i] = "(%)";
        //            gblArrSenMast_Unit_Mod[i] = "(%)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0.01");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("100");

        //            i = 2;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "PRESSURE";
        //            gblArrSenMast_Unit[i] = "(hPa)";
        //            gblArrSenMast_Unit_Mod[i] = "(hPa)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0.1");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("1200");

        //            i = 3;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "ALTITUDE";
        //            gblArrSenMast_Unit[i] = "(%)";
        //            gblArrSenMast_Unit_Mod[i] = "(%)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("1");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("50000");

        //            i = 4;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "HEADING";
        //            gblArrSenMast_Unit[i] = "(Deg)";
        //            gblArrSenMast_Unit_Mod[i] = "(Deg)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("359.99");

        //            i = 5;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "SPEED";
        //            gblArrSenMast_Unit[i] = "(Knot)";
        //            gblArrSenMast_Unit_Mod[i] = "(knot)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("0");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("250");

        //            i = 6;

        //            gblArrSenMast_DefaSenName[i] = "Sensor" + (i + 1).ToString().Trim();
        //            gblArrSenMast_SenName[i] = "DEW POINT";
        //            gblArrSenMast_Unit[i] = "(°C)";
        //            gblArrSenMast_Unit_Mod[i] = "(°C)";
        //            gblArrSenMast_Reso[i] = Convert.ToDouble("0.01");
        //            gblArrSenMast_Min[i] = Convert.ToDouble("-100");
        //            gblArrSenMast_Max[i] = Convert.ToDouble("75");

        //            return true;
        //        }
        //        catch (Exception Ex)
        //        {
        //            strExceptionMessage = Ex.Message;

        //            bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

        //            return false;
        //        }
        //    }
    }
}