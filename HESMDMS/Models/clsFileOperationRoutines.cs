using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class clsFileOperationRoutines
    {
        clsGlobalData clsGD = new clsGlobalData();

        clsExceptionDataRoutine clsEDR = new clsExceptionDataRoutine();

        public string strModuleName = "File Operation Routines";
        public string strFunctionName = "";
        public string strExceptionMessage = "";

        public bool WriteIntoFile(string Location, string FileName, string WriteData)
        {
            strFunctionName = "Write Into File-1";

            lock (this)
            {
                try
                {
                    string NewFile = Location + @"\" + FileName;

                    if (WriteData.Trim().ToUpper() == "REPORT")
                    {
                        //FileStream fs = new FileStream(NewFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        fs.Close();
                    }
                    else
                    {

                        //FileStream fs = new FileStream(NewFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(WriteData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }

                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool WriteIntoFile(string Location, string FileName, string WriteData, string FileHeader)
        {
            strFunctionName = "Write Into File-2";

            lock (this)
            {
                try
                {
                    string NewFile = Location + @"\" + FileName;

                    if (File.Exists(NewFile))
                    {
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(WriteData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }
                    else
                    {
                        FileStream fs = new FileStream(NewFile, FileMode.Append, FileAccess.Write, FileShare.None);

                        String HdrData = FileHeader + System.Environment.NewLine + WriteData;

                        byte[] bytetext = System.Text.Encoding.ASCII.GetBytes(HdrData);

                        fs.Write(bytetext, 0, bytetext.Length);

                        fs.Close();
                    }
                    return true;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public string ReadFromFile(string Location, string FileName)
        {
            strFunctionName = "Read From File";

            lock (this)
            {
                string result = "";
                try
                {
                    string NewFile = Location + @"\" + FileName;

                    FileStream fs = new FileStream(NewFile, FileMode.Open, FileAccess.Read, FileShare.None);

                    byte[] bytetext = new byte[fs.Length];

                    fs.Read(bytetext, 0, bytetext.Length);

                    result = System.Text.Encoding.ASCII.GetString(bytetext);

                    fs.Close();

                    return result;
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return result;
                }
            }
        }


        public bool CopyFile(string SourceFile, string DestinationFile)
        {
            strFunctionName = "Copy File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(SourceFile);

                    if (FileExists == true)
                    {
                        File.Copy(SourceFile, DestinationFile);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool DeleteFile(string FileName)
        {
            strFunctionName = "Delete File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(FileName);

                    if (FileExists == true)
                    {
                        File.Delete(FileName);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public bool RenameFile(string OldFileName, string NewFileName)
        {
            strFunctionName = "Rename File";

            lock (this)
            {
                try
                {
                    bool FileExists = File.Exists(OldFileName);

                    if (FileExists == true)
                    {
                        File.Move(OldFileName, NewFileName);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return false;
                }
            }
        }

        public string FileLocation()
        {
            strFunctionName = "File Location";

            lock (this)
            {
                try
                {
                    return "C:\\";
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return "";
                }
            }
        }

        public string CreateDataFile(string tmpPath, string tmpFileName, string tmpModule)
        {
            strFunctionName = "Create Data File Process";

            lock (this)
            {
                try
                {
                    String DirName = tmpPath.Trim().ToString() + "\\";
                    DirectoryInfo Dir = new DirectoryInfo(DirName);

                    if (!Dir.Exists)
                    {
                        Dir.Create();
                    }

                    if (tmpModule.Trim().ToUpper() == "VALID DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Heading (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Pressure Altitude (m),Vertical Speed (m/s),Ground Distance (Km),Slant Range (Km),Azimuth (Deg),Elevation (Deg),Launch (Sec),Res-3,Res-4,System Date,Data Interface";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Wind Direction (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Pressure Altitude (m),Vertical Speed (m/s),Ground Distance (Km),Slant Range (Km),Azimuth (Deg),Elevation (Deg),Launch (Sec),Res-3,Res-4,System Date,Data Interface";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RECEIVED RAW DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,SOT,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Heading (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Res-3,Res-4,EOT,CRC";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,SOT,UTC Date,UTC Time,Temperature (C),Humidity (%),Pressure (mbar),Int. Temp. (C),Latitude, Latitude Unit,Longitude,Longitude Unit,Status,Satellite,Altitude (m),Wind Direction (Deg),Speed (knot),RSSI,TRFK,Receiver No,Sonde ID,Batt. Volt. (V),Heading (GPS) (Deg),Speed (GPS) (knot),Res-3,Res-4,EOT,CRC";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "GROUND STATION DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Wind Speed(m/s),Wind Direction (Deg),Pressure (mbar),Res-1,Res-2,Res-3";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,UTC Date,UTC Time,Temperature (C),Humidity (%),Wind Speed(knot),Wind Direction (Deg),Pressure (mbar),Res-1,Res-2,Res-3";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "INVALID DATA REPORT")
                    {
                        string tmpHead = "";
                        tmpHead = tmpHead + "Record No,Ref. Sonde ID,Table Name,Remarks,Data Interface,Received Data";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RAOB DATA REPORT")
                    {
                        //string tmpHead = "";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature( °C ),RH( % ), Pressure(millibar),Internal Temperature( °C ),Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude(Metre), Heading(° Due True North), Speed(Knot)";
                        //tmpHead = tmpHead + System.Environment.NewLine;

                        WriteIntoFile(tmpPath, tmpFileName, "");
                        //WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "STANAG DATA REPORT")
                    {
                        string tmpHead = "";
                        tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,RSSI,TRkreg.,Receiver No.";
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }
                    else if (tmpModule.Trim().ToUpper() == "RAW DATA REPORT")
                    {
                        string tmpHead = "";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,RSSI,TRkreg.,Receiver No.,Dew Point";
                        //tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,Dew Point";
                        tmpHead = tmpHead + "UTC Date, UTC Time,Temperature,Humidity, Pressure,Internal Temperature,Latitude, Latitude Unit, Longitude, Longitude Unit,Satellites,Altitude,Altitude Unit, Heading, Heading Unit, Speed, Speed Unit,Dew Point, Derieved Pressure";  ////change on 27-Feb-2016 (as per discussion with Nirajbhai)
                        tmpHead = tmpHead + System.Environment.NewLine;

                        //WriteIntoFile(tmpPath, tmpFileName, "");
                        WriteIntoFile(tmpPath, tmpFileName, tmpHead);
                    }

                    return "OK";
                }
                catch (Exception Ex)
                {
                    strExceptionMessage = Ex.Message;

                    bool ExcpMesg = clsEDR.WriteIntoExceptionFile(strModuleName, strFunctionName, strExceptionMessage, clsGlobalData.strExceptionFileName, clsGlobalData.strLoc_ExceptionFile);

                    return strExceptionMessage;
                }
            }
        }

    }
}