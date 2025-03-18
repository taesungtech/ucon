using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UCON_TEST
{
    public partial class Form1 : Form
    {

        // Modbus

        int[] ModBusSendDataBuf = new int[300];
        int[,] ModBusSendDatabuf2 = new int[10,300];
        int ModBusSendDatabuf_CNT;

        int startData = 0;


        delegate void SetTextEv(string data);

        SetTextEv SetTextInvk2;
        long digital_output_ERR;
        long digital_output_RD;
        long  digital_output_wr;


        long digital_input_ERR;
        long digital_input_RD;
        long digital_input_wr;


        int analog_intput_ERR;
        int analog_intput_ERR1;
        int[] analog_intput_RD = new int[4];
        int[] analog_intput_wr = new int[4];

        int analog_output_ERR;
        int analog_output_ERR1;
        int[] analog_output_RD = new int[4];
        int[] analog_output_wr = new int[4];


        int UART_ERR;

        byte saveID = 11;
        // Data Type
        public static class ModBusSendData
        {
            public static byte saveID = 0;
            public static byte stationNumber = 0;
            public static byte functionCode = 0;
            public static ushort address = 0;
            public static ushort length = 0;
            public static ushort printCheck = 0;
        }
        public static class ModBusReadData
        {
            public static byte saveID = 0;
            public static byte stationNumber = 0;
            public static byte functionCode = 0;
            public static ushort address = 0;
            public static ushort length = 0;
            public static ushort[] data = new ushort[100];
            public static ushort rxPosition = 0;
            public static ushort printCheck = 0;
            public static int T_CYL_Number = 0;
        }

        byte[] rxCrcData = new byte[1024];

        public void ModbusSendData(byte stationNumber, byte functionCode, ushort address, ushort length)
        {
            ushort crc;
            byte[] buf = new byte[30];

            buf[0] = stationNumber;
            buf[1] = functionCode;

            buf[2] = (byte)((address >> 8) & 0x00ff);
            buf[3] = (byte)(address & 0x00ff);

            buf[4] = (byte)((length >> 8) & 0x00ff);
            buf[5] = (byte)(length & 0x00ff);

            crc = ModbusCRC(buf, 6);
            buf[6] = (byte)(crc & 0x00ff);
            buf[7] = (byte)((crc >> 8) & 0x00ff);

            serialPort1.Write(buf, 0, 8);
            TxDataPrint(buf, 8);
            ModBusSendData.stationNumber = stationNumber;
            ModBusSendData.functionCode = functionCode;
            ModBusSendData.address = address;
            ModBusSendData.length = length;

        }


        static ushort ModbusCRC(byte[] nData, ushort wLength)
        {

            ushort[] ModbusCRCTable = new ushort[]   {
                        0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
                        0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
                        0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
                        0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
                        0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
                        0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
                        0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
                        0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
                        0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
                        0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
                        0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
                        0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
                        0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
                        0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
                        0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
                        0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
                        0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
                        0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
                        0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
                        0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
                        0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
                        0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
                        0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
                        0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
                        0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
                        0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
                        0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
                        0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
                        0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
                        0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41, 
                        0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
                        0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040 };
            byte nTemp;

            ushort wCRCWord = 0xFFFF;

            for (ushort i = 0; i < wLength; i++)
            {
                nTemp = (byte)(nData[i] ^ (byte)(wCRCWord));
                wCRCWord >>= 8;
                wCRCWord ^= ModbusCRCTable[nTemp];
            }
            return wCRCWord;
        }


        public void ModbusWriteSendData()
        {
            ushort crc;
            byte[] buf = new byte[300];
            int i = 0;

            buf[0] = (byte)ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 0];
            buf[1] = (byte)ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 1];

            buf[2] = (byte)((ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 2] >> 8) & 0x00ff);
            buf[3] = (byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 2] & 0x00ff);

            buf[4] = (byte)((ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 3] >> 8) & 0x00ff);
            buf[5] = (byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 3] & 0x00ff);

            buf[6] = (byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 3] * 2);

            for (i = 0; i < ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 3]; i++)
            {
                buf[(i * 2) + 7] = (byte)((ModBusSendDatabuf2[ModBusSendDatabuf_CNT,4+i] >> 8) & 0x00ff);
                buf[(i * 2) + 8] = (byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 4+i] & 0x00ff);

            }

            crc = ModbusCRC(buf, (ushort)(buf[6] + 7) );
            buf[(i * 2) + 7] = (byte)(crc & 0x00ff);
            buf[(i * 2) + 8] = (byte)((crc >> 8) & 0x00ff);

            serialPort1.Write(buf, 0, buf[6] + 9);
            TxDataPrint(buf, buf[6] + 9);
            ModBusSendData.stationNumber = (byte)ModBusSendDataBuf[1];
            ModBusSendData.functionCode = (byte)ModBusSendDataBuf[2];
            ModBusSendData.address = (byte)ModBusSendDataBuf[3];
            ModBusSendData.length = (byte)ModBusSendDataBuf[4];



        }

        // TextBox 통신 Rx Data 출력
        public void RxDataPrint(byte[] nData, int len)
        {
            if (checkBox2.Checked)
            {
                    string str2 = "\r\nRx-> ";

                    str2 += (nData[0].ToString("X2") + " ");
                    for (int j = 1; j < len; j++)
                    {
                    str2 += (nData[j].ToString("X2") + " ");
                    }


                    //    textBox1.Invoke(SetTextInvk, str);
                    // listBox1.Invoke(SetTextInvk2, str2);
                    listBox1.Items.Add(str2);
            }

        }


        // TextBox 통신 Tx Data 출력
        public void TxDataPrint(byte[] nData, int len)
        {
            if (checkBox1.Checked)
            {
                string str2 = "\r\nTx-> ";

                 str2 += (nData[0].ToString("X2") + " ");
                  for (int j = 1; j < len; j++)
                  {
                      str2 += (nData[j].ToString("X2") + " ");
                  }

                //    textBox1.Invoke(SetTextInvk, str);
               // listBox1.Invoke(SetTextInvk2, str2);
                listBox1.Items.Add(str2);

            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "connect")
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();
                button1.Text = "disconnect";
            }
            else
            {
                serialPort1.Close();
                button1.Text = "connect";
            }
        }

        private void UCON_107IO_Click(object sender, EventArgs e)
        {

        }
        int rd_cnt;
        private void timer1_Tick(object sender, EventArgs e)
        {

            byte[] buf = new byte[8];
            if (serialPort1.IsOpen)
            {
                if (ModBusSendDatabuf_CNT > 0)
                {
                    


                    ModBusSendDatabuf_CNT --;


                    if (ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 1 ]== 0x06)
                    {
                        ModbusSendData((byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 0]), (byte)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 1]), (ushort)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 2]), (ushort)(ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 3]));

                    }
                    if (ModBusSendDatabuf2[ModBusSendDatabuf_CNT, 1] == 0x10)
                    {
                        ModbusWriteSendData();
                    }
                }
                else
                {
                    //         ModbusSendData(saveID, 3, 0, 55);
                    rd_cnt++;
                    if (rd_cnt == 1) ModbusSendData(11, 4, 0x80, 1);
                    else if (rd_cnt == 2) ModbusSendData(12, 4, 0x80, 1);
                    else if (rd_cnt == 3) ModbusSendData(13, 4, 0x80, 1);
                    else if (rd_cnt == 4) ModbusSendData(14, 4, 0x80, 1);
                    else if (rd_cnt == 5) ModbusSendData(15, 4, 0x82, 8);
                    else
                    {
                        if (tabControl1.SelectedIndex == 0)
                        {
                            ModbusSendData(1, 3, 101, 6);
                        }
                        else if (tabControl1.SelectedIndex == 1)
                        {
                            ModbusSendData(2, 3, 101, 6);
                        }
                        rd_cnt = 0;
                    }
                }
                ModBusSendDataBuf[0] = 0;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            try
            {
                int bytes = serialPort1.BytesToRead;
                byte[] buffer = new byte[bytes];
                serialPort1.Read(buffer, 0, bytes);
                this.Invoke(new MethodInvoker(delegate
                {
                    getFU(buffer, bytes);
                    /*
                    getRxDataSP1 += rs232Utils.ByteArrayToString(buffer);
                    foreach (byte index in buffer)//buffer)
                    {
                        if (index == 10)
                        {
                            getFU(getRxDataSP1);
                            getRxDataSP1 = string.Empty;
                        }
                    }
                     * */

                }));
            }
            catch (Exception err)
            {
                // DisplayStatusbarMessage("Error Opening " + SP1.PortName + " : " + err.Message);
            }
        }


        private void getFU(byte[] rdata, int nLen)
        {

            ushort crc;
            ushort crc_tem;
            //  int nLen = serialPort1.Read(rdata, 0, rdata.Length);

           

            for (int i = 0; i < nLen; i++)
            {
                rxCrcData[ModBusReadData.rxPosition] = rdata[i];

                if (ModBusSendData.functionCode == 0x10)
                {

                    RxDataPrint(rxCrcData, 13);

                    switch (ModBusSendData.functionCode)
                    {
                        case 0:
                            ModBusReadData.stationNumber = rdata[i];
                            if (ModBusSendData.stationNumber == ModBusReadData.stationNumber) ModBusReadData.rxPosition++;
                            else ModBusReadData.rxPosition = 0;


                            if (ModBusReadData.stationNumber == 2) button77.BackColor = Color.LimeGreen;

                            break;
                        case 1:
                            ModBusReadData.functionCode = rdata[i];
                            if (ModBusSendData.functionCode == ModBusReadData.functionCode) ModBusReadData.rxPosition++;
                            else ModBusReadData.rxPosition = 0;
                            break;
                        default:
                            /*
                            if (((ModBusReadData.rxPosition - 3) % 2) == 0) ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2] = rdata[i];
                            else
                            {
                                ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2] = (ushort)(((ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2]) << 8) + rdata[i]);
                            }
                            */
                            ModBusReadData.rxPosition++;

                            break;
                    }

                    if (ModBusReadData.rxPosition >= (13))
                    {
                        RxDataPrint(rxCrcData, 13);
                        ModBusReadData.rxPosition = 0;
                        /*
                        crc = ModbusCRC(rxCrcData, (ushort)((ModBusReadData.length) + 3));

                        crc_tem = rxCrcData[(ushort)((ModBusReadData.length) + 4)];
                        crc_tem <<= 8;
                        crc_tem += rxCrcData[(ushort)((ModBusReadData.length) + 3)];

                        if (crc == crc_tem)
                        {
                            RxDataPrint(rxCrcData, ((ModBusReadData.length) + 5));
                            ModBusReadData.address = ModBusSendData.address;
                            ModBusDataRead();
                        }
                        else
                        {
                            string str3 = "\r\nRx-> CRC error ";
                            str3 += (crc.ToString("X04") + " ");
                            str3 += (crc_tem.ToString("X04") + " ");

                            textBox1.Invoke(SetTextInvk2, str3);
                        }
                        ModBusReadData.rxPosition = 0;
                         * */
                    }

                }
                else
                {

                    switch (ModBusReadData.rxPosition)
                    {
                        case 0:
                            ModBusReadData.stationNumber = rdata[i];
                            if (ModBusSendData.stationNumber == ModBusReadData.stationNumber) ModBusReadData.rxPosition++;
                            else ModBusReadData.rxPosition = 0;


                            if (ModBusReadData.stationNumber == 1) button76.BackColor = Color.LimeGreen;
                            if (ModBusReadData.stationNumber == 2) button77.BackColor = Color.LimeGreen;
                            break;
                        case 1:
                            ModBusReadData.functionCode = rdata[i];
                            if (ModBusSendData.functionCode == ModBusReadData.functionCode) ModBusReadData.rxPosition++;
                            else ModBusReadData.rxPosition = 0;
                            break;
                        case 2:
                            ModBusReadData.length = rdata[i];
                            if (ModBusSendData.length == (ModBusReadData.length / 2)) ModBusReadData.rxPosition++;
                            else ModBusReadData.rxPosition = 0;

                            break;
                        default:
                            if (((ModBusReadData.rxPosition - 3) % 2) == 0) ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2] = rdata[i];
                            else
                            {
                                ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2] = (ushort)(((ModBusReadData.data[(ModBusReadData.rxPosition - 3) / 2]) << 8) + rdata[i]);
                            }

                            ModBusReadData.rxPosition++;

                            break;
                    }

                    if (ModBusReadData.rxPosition >= ((ModBusReadData.length) + 5))
                    {


                        crc = ModbusCRC(rxCrcData, (ushort)((ModBusReadData.length) + 3));

                        crc_tem = rxCrcData[(ushort)((ModBusReadData.length) + 4)];
                        crc_tem <<= 8;
                        crc_tem += rxCrcData[(ushort)((ModBusReadData.length) + 3)];

                        if (crc == crc_tem)
                        {

                            RxDataPrint(rxCrcData, ((ModBusReadData.length) + 5));
                            ModBusReadData.address = ModBusSendData.address;
                            ModBusDataRead();


                        }
                        else
                        {
                            /*
                            string str3 = "\r\nRx-> CRC error ";
                            str3 += (crc.ToString("X04") + " ");
                            str3 += (crc_tem.ToString("X04") + " ");

                            textBox1.Invoke(SetTextInvk2, str3);
                            */
                        }
                        ModBusReadData.rxPosition = 0;
                    }

                }

            }
        }



        char led_buf;
        private void ModBusDataRead()
        {
            short subuffer;



            for (ushort i = 0; i < (ModBusReadData.length / 2); i++)
            {
                if (checkBox2.Checked)
                {
                    string str2 = "\r\nRx ADDR-> ";

                    str2 += ((ModBusReadData.address + i).ToString() );
                 
                   // listBox1.Items.Add(str2);
                }
                switch ((ModBusReadData.address + i))
                {

                    case 0: // 

                        break;
                    case 1: //   
                        
                         
                        break;
                    case 0x80: // 

                        if (ModBusReadData.stationNumber == 14)
                        {
                             digital_output_RD = ((digital_output_RD & 0xfffffffffe) | ((ModBusReadData.data[i]) & 0x0001)) & 0xffffffffff;                        
                        }
                        else if (ModBusReadData.stationNumber == 11)
                        {
                            digital_output_RD = ((digital_output_RD & 0xfffffe0001) | ((ModBusReadData.data[i]) << 1)) & 0xffffffffff;
                        }
                        else if (ModBusReadData.stationNumber == 12)
                        {
                            digital_output_RD = ((digital_output_RD & 0xfe0001ffff) | ((long)(ModBusReadData.data[i]) << 17)) & 0xffffffffff;
                        }
                        else if (ModBusReadData.stationNumber == 13)
                        {
                            digital_output_RD = ((digital_output_RD & 0x01ffffffff) | ((long)(ModBusReadData.data[i]) << 33)) & 0xffffffffff;
                        }

                        if ((digital_output_ERR & 0x001) == 0x0001) button22.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x001) == 0x0001) button22.BackColor = Color.LimeGreen;
                        else button22.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x002) == 0x002) button23.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x002) == 0x0002) button23.BackColor = Color.LimeGreen;
                        else button23.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x004) == 0x004) button24.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x004) == 0x0004) button24.BackColor = Color.LimeGreen;
                        else button24.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x008) == 0x008) button25.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x008) == 0x0008) button25.BackColor = Color.LimeGreen;
                        else button25.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0010) == 0x0010) button26.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0010) == 0x0010) button26.BackColor = Color.LimeGreen;
                        else button26.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0020) == 0x0020) button27.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0020) == 0x0020) button27.BackColor = Color.LimeGreen;
                        else button27.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0040) == 0x0040) button28.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0040) == 0x0040) button28.BackColor = Color.LimeGreen;
                        else button28.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0080) == 0x0080) button29.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0080) == 0x0080) button29.BackColor = Color.LimeGreen;
                        else button29.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0100) == 0x0100) button30.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0100) == 0x0100) button30.BackColor = Color.LimeGreen;
                        else button30.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0200) == 0x0200) button31.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0200) == 0x0200) button31.BackColor = Color.LimeGreen;
                        else button31.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0400) == 0x0400) button32.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0400) == 0x0400) button32.BackColor = Color.LimeGreen;
                        else button32.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0800) == 0x0800) button33.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0800) == 0x0800) button33.BackColor = Color.LimeGreen;
                        else button33.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x1000) == 0x1000) button34.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x1000) == 0x1000) button34.BackColor = Color.LimeGreen;
                        else button34.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x2000) == 0x2000) button35.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x2000) == 0x2000) button35.BackColor = Color.LimeGreen;
                        else button35.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x4000) == 0x4000) button36.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x4000) == 0x4000) button36.BackColor = Color.LimeGreen;
                        else button36.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x8000) == 0x8000) button37.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x8000) == 0x8000) button37.BackColor = Color.LimeGreen;
                        else button37.BackColor = System.Drawing.Color.Gainsboro;



                        if ((digital_output_ERR & 0x0010000) == 0x0010000) button38.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0010000) == 0x00010000) button38.BackColor = Color.LimeGreen;
                        else button38.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0020000) == 0x0020000) button39.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0020000) == 0x00020000) button39.BackColor = Color.LimeGreen;
                        else button39.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0040000) == 0x0040000) button40.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0040000) == 0x00040000) button40.BackColor = Color.LimeGreen;
                        else button40.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0080000) == 0x0080000) button41.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0080000) == 0x00080000) button41.BackColor = Color.LimeGreen;
                        else button41.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x00100000) == 0x00100000) button42.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x00100000) == 0x00100000) button42.BackColor = Color.LimeGreen;
                        else button42.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x00200000) == 0x00200000) button43.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x00200000) == 0x00200000) button43.BackColor = Color.LimeGreen;
                        else button43.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x00400000) == 0x00400000) button44.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x00400000) == 0x00400000) button44.BackColor = Color.LimeGreen;
                        else button44.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x00800000) == 0x00800000) button45.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x00800000) == 0x00800000) button45.BackColor = Color.LimeGreen;
                        else button45.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x01000000) == 0x01000000) button46.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x01000000) == 0x01000000) button46.BackColor = Color.LimeGreen;
                        else button46.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x02000000) == 0x02000000) button47.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x02000000) == 0x02000000) button47.BackColor = Color.LimeGreen;
                        else button47.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x04000000) == 0x04000000) button48.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x04000000) == 0x04000000) button48.BackColor = Color.LimeGreen;
                        else button48.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x08000000) == 0x08000000) button49.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x08000000) == 0x08000000) button49.BackColor = Color.LimeGreen;
                        else button49.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x10000000) == 0x10000000) button50.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x10000000) == 0x10000000) button50.BackColor = Color.LimeGreen;
                        else button50.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x20000000) == 0x20000000) button51.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x20000000) == 0x20000000) button51.BackColor = Color.LimeGreen;
                        else button51.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x40000000) == 0x40000000) button52.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x40000000) == 0x40000000) button52.BackColor = Color.LimeGreen;
                        else button52.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x80000000) == 0x80000000) button53.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x80000000) == 0x80000000) button53.BackColor = Color.LimeGreen;
                        else button53.BackColor = System.Drawing.Color.Gainsboro;


                        if ((digital_output_ERR & 0x0100000000) == 0x0100000000) button54.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0100000000) == 0x0100000000) button54.BackColor = Color.LimeGreen;
                        else button54.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0200000000) == 0x0200000000) button55.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0200000000) == 0x0200000000) button55.BackColor = Color.LimeGreen;
                        else button55.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0400000000) == 0x0400000000) button56.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0400000000) == 0x0400000000) button56.BackColor = Color.LimeGreen;
                        else button56.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x0800000000) == 0x0800000000) button57.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x0800000000) == 0x0800000000) button57.BackColor = Color.LimeGreen;
                        else button57.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x1000000000) == 0x1000000000) button58.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x1000000000) == 0x1000000000) button58.BackColor = Color.LimeGreen;
                        else button58.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x2000000000) == 0x2000000000) button59.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x2000000000) == 0x2000000000) button59.BackColor = Color.LimeGreen;
                        else button59.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x4000000000) == 0x4000000000) button60.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x4000000000) == 0x4000000000) button60.BackColor = Color.LimeGreen;
                        else button60.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_output_ERR & 0x8000000000) == 0x8000000000) button61.BackColor = Color.Red;
                        else if ((digital_output_RD & 0x8000000000) == 0x8000000000) button61.BackColor = Color.LimeGreen;
                        else button61.BackColor = System.Drawing.Color.Gainsboro;


                        button2.BackColor = button22.BackColor;
                        button3.BackColor = button23.BackColor;
                        button4.BackColor = button24.BackColor;
                        button5.BackColor = button25.BackColor;
                        button6.BackColor = button26.BackColor;
                        button7.BackColor = button27.BackColor;
                        button8.BackColor = button28.BackColor;
                        button9.BackColor = button29.BackColor;
                        button10.BackColor = button30.BackColor;
                        button11.BackColor = button31.BackColor;
                        button12.BackColor = button32.BackColor;
                        button13.BackColor = button33.BackColor;
                        button14.BackColor = button34.BackColor;
                        button15.BackColor = button35.BackColor;
                        button16.BackColor = button36.BackColor;
                        button17.BackColor = button37.BackColor;
                        button18.BackColor = button38.BackColor;
                        button19.BackColor = button39.BackColor;
                        button20.BackColor = button40.BackColor;
                        button21.BackColor = button41.BackColor;



                        break;

                    case 101: // 
                        
                        digital_input_RD = ModBusReadData.data[i];


                        if ((digital_input_ERR & 0x001) == 0x0001) button62.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x001) == 0x0001) button62.BackColor = Color.LimeGreen;
                        else button62.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_input_ERR & 0x002) == 0x0002) button63.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x002) == 0x0002) button63.BackColor = Color.LimeGreen;
                        else button63.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_input_ERR & 0x004) == 0x0004) button64.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x004) == 0x0004) button64.BackColor = Color.LimeGreen;
                        else button64.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_input_ERR & 0x008) == 0x0008) button65.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x008) == 0x0008) button65.BackColor = Color.LimeGreen;
                        else button65.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_input_ERR & 0x0010) == 0x00010) button66.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x0010) == 0x00010) button66.BackColor = Color.LimeGreen;
                        else button66.BackColor = System.Drawing.Color.Gainsboro;

                        if ((digital_input_ERR & 0x0020) == 0x00020) button67.BackColor = Color.Red;
                        else if ((digital_input_RD & 0x0020) == 0x00020) button67.BackColor = Color.LimeGreen;
                        else button67.BackColor = System.Drawing.Color.Gainsboro;

                        button68.BackColor = button62.BackColor;
                        button69.BackColor = button63.BackColor;
                        button70.BackColor = button64.BackColor;
                        button71.BackColor = button65.BackColor;
                        button79.BackColor = button66.BackColor;
                        button75.BackColor = button67.BackColor;


                        break;
                    case 102: // 
                        analog_intput_RD[0] = ModBusReadData.data[i];
                        textBox13.Text = Convert.ToString((short)((ModBusReadData.data[i] )));


                        if ((analog_intput_ERR & 0x001) == 0x0001) textBox13.BackColor = Color.Red;
                        else textBox13.BackColor = Color.LimeGreen;

                        textBox1.BackColor = textBox13.BackColor;
                        textBox1.Text = textBox13.Text;


                        break;
                    case 103: // 
                        analog_intput_RD[1] = ModBusReadData.data[i];
                        textBox14.Text = Convert.ToString((short)((ModBusReadData.data[i])));


                        if ((analog_intput_ERR & 0x002) == 0x0002) textBox14.BackColor = Color.Red;
                        else textBox14.BackColor = Color.LimeGreen;

                        textBox2.BackColor = textBox14.BackColor;
                        textBox2.Text = textBox14.Text;

                        break;
                    case 104: // 
                        analog_intput_RD[2] = ModBusReadData.data[i];
                        textBox15.Text = Convert.ToString((short)((ModBusReadData.data[i])));

                        if ((analog_intput_ERR & 0x004) == 0x0004) textBox15.BackColor = Color.Red;
                        else textBox15.BackColor = Color.LimeGreen;

                        textBox3.BackColor = textBox15.BackColor;
                        textBox3.Text = textBox15.Text;


                        break;
                    case 105: // 
                        analog_intput_RD[3] = ModBusReadData.data[i];
                        textBox16.Text = Convert.ToString((short)((ModBusReadData.data[i])));


                        if ((analog_intput_ERR & 0x008) == 0x0008) textBox16.BackColor = Color.Red;
                        else textBox16.BackColor = Color.LimeGreen;

                        textBox4.BackColor = textBox16.BackColor;
                        textBox4.Text = textBox16.Text;


                        break;

                    case 106: // 

                        UART_ERR = ModBusReadData.data[i];

                        if ((UART_ERR & 0x01) == 0x0001) button92.BackColor = Color.LimeGreen;
                        else button92.BackColor = Color.Red;

                        if ((UART_ERR & 0x002) == 0x0002) button91.BackColor = Color.LimeGreen;
                        else button91.BackColor = Color.Red;

                        if ((UART_ERR & 0x004) == 0x0004) button90.BackColor = Color.LimeGreen;
                        else button90.BackColor = Color.Red;

                        if ((UART_ERR & 0x008) == 0x0008) button89.BackColor = Color.LimeGreen;
                        else button89.BackColor = Color.Red;

                        if ((UART_ERR & 0x010) == 0x0010) button88.BackColor = Color.LimeGreen;
                        else button88.BackColor = Color.Red;


                        break;

                    case 0x82: // 
                        analog_output_RD[0] = (ModBusReadData.data[i] * 10000) / 65535;   
                        textBox9.Text = Convert.ToString((short)( (ModBusReadData.data[i]* 10000) / 65535)     );


                        if ((analog_output_ERR & 0x001) == 0x0001) textBox9.BackColor = Color.Red;
                        else textBox9.BackColor = Color.LimeGreen;

                        textBox7.Text = (ModBusReadData.data[i]).ToString();

                        textBox5.BackColor = textBox9.BackColor;
                        textBox5.Text = textBox9.Text;
                        break;

                    case 0x83: // 
                        analog_output_RD[1] = (ModBusReadData.data[i] * 10000) / 65535;
                        textBox10.Text = Convert.ToString((short)((ModBusReadData.data[i] * 10000) / 65535) );


                        if ((analog_output_ERR & 0x002) == 0x0002) textBox10.BackColor = Color.Red;
                        else textBox10.BackColor = Color.LimeGreen;


                        textBox8.Text = (digital_output_RD).ToString();

                        textBox6.BackColor = textBox10.BackColor;
                        textBox6.Text = textBox10.Text;


                        break;

                    case 0x84: // 
                        analog_output_RD[2] = (ModBusReadData.data[i] * 10000) / 65535;
                        textBox11.Text = Convert.ToString((short)((ModBusReadData.data[i] * 10000) / 65535));


                        if ((analog_output_ERR & 0x004) == 0x0004) textBox11.BackColor = Color.Red;
                        else textBox11.BackColor = Color.LimeGreen;


                        break;

                    case 0x85: // 
                        analog_output_RD[3] = (ModBusReadData.data[i] * 10000) / 65535;
                        textBox12.Text = Convert.ToString((short)((ModBusReadData.data[i] * 10000) / 65535));

                        if ((analog_output_ERR & 0x008) == 0x0008) textBox12.BackColor = Color.Red;
                        else textBox12.BackColor = Color.LimeGreen;
                        break;

                    case 0x87: // 
                        textBox5.Text = Convert.ToString((short)((ModBusReadData.data[i] * 10000) / 65535));
                        break;

                    case 0x88: // 
                        textBox6.Text = Convert.ToString((short)((ModBusReadData.data[i] * 10000) / 65535));
                        break;

                    default:
                        break;

                }
            }
        }

        private void button72_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 0)
            {

                ModBusSendDatabuf2[0, 0] = 13;
                ModBusSendDatabuf2[0, 1] = 6;
                ModBusSendDatabuf2[0, 2] = 0;
                ModBusSendDatabuf2[0, 3] = (int)(digital_input_wr);  // DI data

                ModBusSendDatabuf2[1, 0] = 14;
                ModBusSendDatabuf2[1, 1] = 16;
                ModBusSendDatabuf2[1, 2] = 2;
                ModBusSendDatabuf2[1, 3] = 4;
                ModBusSendDatabuf2[1, 4] = analog_intput_wr[0];  //AO1 data
                ModBusSendDatabuf2[1, 5] = analog_intput_wr[1];  //AO2 data
                ModBusSendDatabuf2[1, 6] = analog_intput_wr[2];  //AO3 data
                ModBusSendDatabuf2[1, 7] = analog_intput_wr[3];  //AO4 data


                ModBusSendDatabuf2[2, 0] = 2;
                ModBusSendDatabuf2[2, 1] = 16;
                ModBusSendDatabuf2[2, 2] = 500;
                ModBusSendDatabuf2[2, 3] = 6;
                ModBusSendDatabuf2[2, 4] = (int)(digital_output_wr >> 16);  //DO1 data
                ModBusSendDatabuf2[2, 5] = (int)(digital_output_wr);  //DO2 data
                ModBusSendDatabuf2[2, 6] = analog_output_wr[0];  //AO1 data
                ModBusSendDatabuf2[2, 7] = analog_output_wr[1];  //AO2 data
                ModBusSendDatabuf2[2, 8] = analog_output_wr[2];  //AO3 data
                ModBusSendDatabuf2[2, 9] = analog_output_wr[3];  //AO4 data




                ModBusSendDatabuf_CNT = 3;
            }
            if (tabControl1.SelectedIndex == 1)
            {
                ModBusSendDatabuf2[0, 0] = 13;
                ModBusSendDatabuf2[0, 1] = 6;
                ModBusSendDatabuf2[0, 2] = 0;
                ModBusSendDatabuf2[0, 3] = (int)(digital_input_wr);  // DI data

                ModBusSendDatabuf2[1, 0] = 14;
                ModBusSendDatabuf2[1, 1] = 16;
                ModBusSendDatabuf2[1, 2] = 2;
                ModBusSendDatabuf2[1, 3] = 4;
                ModBusSendDatabuf2[1, 4] = analog_intput_wr[0];  //AO1 data
                ModBusSendDatabuf2[1, 5] = analog_intput_wr[1];  //AO2 data
                ModBusSendDatabuf2[1, 6] = analog_intput_wr[2];  //AO3 data
                ModBusSendDatabuf2[1, 7] = analog_intput_wr[3];  //AO4 data


                ModBusSendDatabuf2[2, 0] = 2;
                ModBusSendDatabuf2[2, 1] = 16;
                ModBusSendDatabuf2[2, 2] = 500;
                ModBusSendDatabuf2[2, 3] = 7;
                ModBusSendDatabuf2[2, 4] = (int)(digital_output_wr >> 32);  //DO1 data
                ModBusSendDatabuf2[2, 5] = (int)(digital_output_wr >> 16);  //DO1 data
                ModBusSendDatabuf2[2, 6] = (int)(digital_output_wr);  //DO2 data
                ModBusSendDatabuf2[2, 7] = analog_output_wr[0];  //AO1 data
                ModBusSendDatabuf2[2, 8] = analog_output_wr[1];  //AO2 data
                ModBusSendDatabuf2[2, 9] = analog_output_wr[2];  //AO3 data
                ModBusSendDatabuf2[2, 10] = analog_output_wr[3];  //AO4 data


                ModBusSendDatabuf_CNT = 3;

            }

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

    /*
        int analog_intput_ERR;
        int[] analog_intput_RD = new int[4];
        int[] analog_intput_wr = new int[4];

        int analog_output_ERR;
        int[] analog_output_RD = new int[4];
        int[] analog_output_wr = new int[4];

    */
        private void ADC_check1( )
        {
            analog_intput_ERR1 = 0 ;
            for (int i = 0; i < 4; i++)
            {
                if ((analog_intput_RD[i] - 300) > analog_intput_wr[i])
                {
                    analog_intput_ERR1 = analog_intput_ERR1 | (0x001 << i);
                }
                else if ((analog_intput_RD[i] + 300) < analog_intput_wr[i])
                {
                    analog_intput_ERR1 = analog_intput_ERR1 | (0x001 << i);
                }
                else
                {
                    

                }
                analog_intput_ERR = analog_intput_ERR | analog_intput_ERR1;
            }
        }
        private void ADC_check2()
        {
            int buf;
            analog_output_ERR1 = 0;
            if (tabControl1.SelectedIndex == 1)
            {

              
                buf = Convert.ToInt32(textBox5.Text);

                
               // textBox25.Text = analog_output_RD[1].ToString();

                if (analog_output_wr[1] == 0)
                {
                    if (buf > 300)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }

                }
                else if (analog_output_wr[1] == 10000)
                {
                    if (buf < 300)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }
                    else if (buf > 700)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }

                }
                else if (analog_output_wr[1] == 30000)
                {
                    if (buf < 2100)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }
                    else if (buf > 2500)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }


                }
                else if (analog_output_wr[1] == 50000)
                {
                    if (buf < 4000)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }
                    else if (buf > 4500)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 0);
                    }


                }

                buf = Convert.ToInt32(textBox6.Text);

                if (analog_output_wr[0] == 0)
                {
                    if (buf > 300)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }
                }
                else if (analog_output_wr[0] == 10000)
                {
                    if (buf < 300)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }
                    else if (buf  > 700)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 10);
                    }
                }
                else if (analog_output_wr[0] == 30000)
                {
                    if (buf < 2100)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }
                    else if (buf > 2500)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }

                }
                else if (analog_output_wr[0] == 50000)
                {
                    if (buf < 4000)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }
                    else if (buf > 4500)
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << 1);
                    }

                }

                analog_output_ERR = analog_output_ERR | analog_output_ERR1;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((analog_output_RD[i] - 300) > analog_output_wr[i])
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << i);
                    }
                    else if ((analog_output_RD[i] + 300) < analog_output_wr[i])
                    {
                        analog_output_ERR1 = analog_output_ERR1 | (0x001 << i);
                    }
                    else
                    {


                    }
                    analog_output_ERR = analog_output_ERR | analog_output_ERR1;
                }

            }
            
        }

        int check_CNT = 0;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                check_CNT++;
                if (check_CNT == 1)
                {
                    digital_output_ERR = 0;
                    digital_output_wr = 0x11111;
                    digital_input_ERR = 0;
                    digital_input_wr = 0;

                    analog_intput_ERR = 0;
                    analog_intput_wr[0] = 10000;
                    analog_intput_wr[1] =  6000;
                    analog_intput_wr[2] =  3000;
                    analog_intput_wr[3] = 0;

                    analog_output_ERR = 0;
                    analog_output_wr[0] = 10000;
                    analog_output_wr[1] =  6000;
                    analog_output_wr[2] =  3000;
                    analog_output_wr[3] = 0;


                    panel2.BackColor = Color.White;
                    panel9.BackColor = Color.White;
                    panel8.BackColor = Color.White;
                    panel7.BackColor = Color.White;
                    label21.Text = "---";
                    label23.Text = "---";
                    label27.Text = "---";
                    label33.Text = "---";



                    panel11.BackColor = Color.White;
                    panel14.BackColor = Color.White;
                    panel13.BackColor = Color.White;
                    panel12.BackColor = Color.White;
                    label22.Text = "---";
                    label30.Text = "---";
                    label26.Text = "---";
                    label32.Text = "---";



                    panel16.BackColor = Color.White;
                    panel19.BackColor = Color.White;
                    panel18.BackColor = Color.White;
                    panel17.BackColor = Color.White;
                    label25.Text = "---";
                    label29.Text = "---";
                    label35.Text = "---";
                    label31.Text = "---";



                    panel21.BackColor = Color.White;
                    panel24.BackColor = Color.White;
                    panel23.BackColor = Color.White;
                    panel22.BackColor = Color.White;
                    label24.Text = "---";
                    label28.Text = "---";
                    label34.Text = "---";
                    label36.Text = "---";

                    textBox13.BackColor = Color.White;
                    textBox14.BackColor = Color.White;
                    textBox15.BackColor = Color.White;
                    textBox16.BackColor = Color.White;

                    textBox9.BackColor = Color.White;
                    textBox10.BackColor = Color.White;
                    textBox11.BackColor = Color.White;
                    textBox12.BackColor = Color.White;


                    button76.BackColor = Color.Gainsboro;
                    button77.BackColor = Color.Gainsboro;
                }
                else if (check_CNT == 2)
                {
                    digital_output_ERR = ( digital_output_ERR | (digital_output_RD ^ digital_output_wr) ) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();

                    if ((digital_output_RD ^ digital_output_wr) == 0)
                    {
                        panel2.BackColor = Color.LimeGreen;
                        label21.Text = "PASSED";
                    }
                    else
                    {
                        panel2.BackColor = Color.Red;
                        label21.Text = "FAILURE";
                    }


                    if (((digital_input_RD ^ digital_input_wr)& 0x3f ) == 0)
                    {
                        panel11.BackColor = Color.LimeGreen;
                        label22.Text = "PASSED";
                    }
                    else
                    {
                        panel11.BackColor = Color.Red;
                        label22.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel16.BackColor = Color.LimeGreen;
                        label25.Text = "PASSED";
                    }
                    else
                    {
                        panel16.BackColor = Color.Red;
                        label25.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel21.BackColor = Color.LimeGreen;
                        label24.Text = "PASSED";
                    }
                    else
                    {
                        panel21.BackColor = Color.Red;
                        label24.Text = "FAILURE";
                    }



                    digital_output_wr = 0x22222;
                    digital_input_wr = 0x55;

                    analog_intput_wr[0] = 0;
                    analog_intput_wr[1] = 10000;
                    analog_intput_wr[2] = 6000;
                    analog_intput_wr[3] = 3000;

                    analog_output_wr[0] = 0;
                    analog_output_wr[1] = 10000;
                    analog_output_wr[2] = 6000;
                    analog_output_wr[3] = 3000;


                }
                else if (check_CNT == 3)
                {

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if ((digital_output_RD ^ digital_output_wr) == 0)
                    {
                        panel9.BackColor = Color.LimeGreen;

                        label23.Text = "PASSED";
                    }
                    else
                    {
                        panel9.BackColor = Color.Red;
                        label23.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel14.BackColor = Color.LimeGreen;
                        label30.Text = "PASSED";
                    }
                    else
                    {
                        panel14.BackColor = Color.Red;
                        label30.Text = "FAILURE";
                    }



                    if (analog_intput_ERR1 == 0)
                    {
                        panel19.BackColor = Color.LimeGreen;
                        label29.Text = "PASSED";
                    }
                    else
                    {
                        panel19.BackColor = Color.Red;
                        label29.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel24.BackColor = Color.LimeGreen;
                        label28.Text = "PASSED";
                    }
                    else
                    {
                        panel24.BackColor = Color.Red;
                        label28.Text = "FAILURE";
                    }



                    digital_output_wr = 0x44444;
                    digital_input_wr= 0x2A;

                    analog_intput_wr[0] = 3000;
                    analog_intput_wr[1] = 0;
                    analog_intput_wr[2] = 10000;
                    analog_intput_wr[3] = 6000;

                    analog_output_wr[0] = 3000;
                    analog_output_wr[1] = 0;
                    analog_output_wr[2] = 10000;
                    analog_output_wr[3] = 6000;

                }
                else if (check_CNT == 4)
                {

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr) ) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr)  & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if ((digital_output_RD ^ digital_output_wr) == 0)
                    {
                        panel8.BackColor = Color.LimeGreen;
                        label27.Text = "PASSED";
                    }
                    else
                    {
                        panel8.BackColor = Color.Red;
                        label27.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel13.BackColor = Color.LimeGreen;
                        label26.Text = "PASSED";
                    }
                    else
                    {
                        panel13.BackColor = Color.Red;
                        label26.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel18.BackColor = Color.LimeGreen;
                        label35.Text = "PASSED";
                    }
                    else
                    {
                        panel18.BackColor = Color.Red;
                        label35.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel23.BackColor = Color.LimeGreen;
                        label34.Text = "PASSED";
                    }
                    else
                    {
                        panel23.BackColor = Color.Red;
                        label34.Text = "FAILURE";
                    }


                    digital_output_wr = 0x88888;
                    digital_input_wr = 0x7f;

                    analog_intput_wr[0] = 6000;
                    analog_intput_wr[1] = 3000;
                    analog_intput_wr[2] = 0;
                    analog_intput_wr[3] = 10000;

                    analog_output_wr[0] = 6000;
                    analog_output_wr[1] = 3000;
                    analog_output_wr[2] = 0;
                    analog_output_wr[3] = 10000;

                }
                else
                {
                    check_CNT = 0;

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if ((digital_output_RD ^ digital_output_wr) == 0)
                    {
                        panel7.BackColor = Color.LimeGreen;
                        label33.Text = "PASSED";
                    }
                    else
                    {
                        panel7.BackColor = Color.Red;
                        label33.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel12.BackColor = Color.LimeGreen;
                        label32.Text = "PASSED";
                    }
                    else
                    {
                        panel12.BackColor = Color.Red;
                        label32.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel17.BackColor = Color.LimeGreen;
                        label31.Text = "PASSED";
                    }
                    else
                    {
                        panel17.BackColor = Color.Red;
                        label31.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel22.BackColor = Color.LimeGreen;
                        label36.Text = "PASSED";
                    }
                    else
                    {
                        panel22.BackColor = Color.Red;
                        label36.Text = "FAILURE";
                    }
                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                check_CNT++;
                if (check_CNT == 1)
                {
                    digital_output_ERR = 0;
                    digital_output_wr =   0x1111111111;
//                    digital_output_wr = 0xffffffffff;
                    //                 digital_output_wr = 0x1;
                    //                 digital_output_wr = 0x1;
                    digital_input_ERR = 0;
                    digital_input_wr = 0;

                    analog_intput_ERR = 0;
                    analog_intput_wr[0] = 10000;
                    analog_intput_wr[1] = 6000;
                    analog_intput_wr[2] = 3000;
                    analog_intput_wr[3] = 0;

                    analog_output_ERR = 0;
                    analog_output_wr[0] = 50000;
                    analog_output_wr[1] = 30000;
                    analog_output_wr[2] = 10000;
                    analog_output_wr[3] = 0;


                    panel40.BackColor = Color.White;
                    panel35.BackColor = Color.White;
                    panel30.BackColor = Color.White;
                    panel1.BackColor = Color.White;
                    label56.Text = "---";
                    label51.Text = "---";
                    label46.Text = "---";
                    label41.Text = "---";



                    panel43.BackColor = Color.White;
                    panel38.BackColor = Color.White;
                    panel33.BackColor = Color.White;
                    panel28.BackColor = Color.White;
                    label59.Text = "---";
                    label54.Text = "---";
                    label49.Text = "---";
                    label44.Text = "---";



                    panel42.BackColor = Color.White;
                    panel37.BackColor = Color.White;
                    panel32.BackColor = Color.White;
                    panel27.BackColor = Color.White;
                    label58.Text = "---";
                    label53.Text = "---";
                    label48.Text = "---";
                    label43.Text = "---";



                    panel41.BackColor = Color.White;
                    panel36.BackColor = Color.White;
                    panel31.BackColor = Color.White;
                    panel26.BackColor = Color.White;
                    label57.Text = "---";
                    label52.Text = "---";
                    label47.Text = "---";
                    label42.Text = "---";

                    textBox1.BackColor = Color.White;
                    textBox2.BackColor = Color.White;
                    textBox3.BackColor = Color.White;
                    textBox4.BackColor = Color.White;

                    textBox5.BackColor = Color.White;
                    textBox6.BackColor = Color.White;


                }
                else if (check_CNT == 2)
                {
                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();

                    if (((digital_output_RD ^ digital_output_wr) & 0x00000fffff) == 0)
                    {
                        panel40.BackColor = Color.LimeGreen;
                        label56.Text = "PASSED";
                    }
                    else
                    {
                        panel40.BackColor = Color.Red;
                        label56.Text = "FAILURE";
                    }


                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel35.BackColor = Color.LimeGreen;
                        label51.Text = "PASSED";
                    }
                    else
                    {
                        panel35.BackColor = Color.Red;
                        label51.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel30.BackColor = Color.LimeGreen;
                        label46.Text = "PASSED";
                    }
                    else
                    {
                        panel30.BackColor = Color.Red;
                        label46.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel1.BackColor = Color.LimeGreen;
                        label41.Text = "PASSED";
                    }
                    else
                    {
                        panel1.BackColor = Color.Red;
                        label41.Text = "FAILURE";
                    }



                    digital_output_wr = 0x2222222222;
   //                 digital_output_wr = 0x2;
                    digital_input_wr = 0x55;

                    analog_intput_wr[0] = 0;
                    analog_intput_wr[1] = 10000;
                    analog_intput_wr[2] = 6000;
                    analog_intput_wr[3] = 3000;

                    analog_output_wr[0] = 0;
                    analog_output_wr[1] = 50000;
                    analog_output_wr[2] = 30000;
                    analog_output_wr[3] = 10000;


                }
                else if (check_CNT == 3)
                {

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if (((digital_output_RD ^ digital_output_wr) & 0x00000fffff) == 0)
                    {
                        panel43.BackColor = Color.LimeGreen;

                        label59.Text = "PASSED";
                    }
                    else
                    {
                        panel43.BackColor = Color.Red;
                        label59.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel38.BackColor = Color.LimeGreen;
                        label54.Text = "PASSED";
                    }
                    else
                    {
                        panel38.BackColor = Color.Red;
                        label54.Text = "FAILURE";
                    }



                    if (analog_intput_ERR1 == 0)
                    {
                        panel33.BackColor = Color.LimeGreen;
                        label49.Text = "PASSED";
                    }
                    else
                    {
                        panel33.BackColor = Color.Red;
                        label49.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel28.BackColor = Color.LimeGreen;
                        label44.Text = "PASSED";
                    }
                    else
                    {
                        panel28.BackColor = Color.Red;
                        label44.Text = "FAILURE";
                    }



                    digital_output_wr = 0x4444444444;
  //                  digital_output_wr = 0x4;
                    digital_input_wr = 0x2A;

                    analog_intput_wr[0] = 3000;
                    analog_intput_wr[1] = 0;
                    analog_intput_wr[2] = 10000;
                    analog_intput_wr[3] = 6000;

                    analog_output_wr[0] = 10000;
                    analog_output_wr[1] = 0;
                    analog_output_wr[2] = 50000;
                    analog_output_wr[3] = 30000;

                }
                else if (check_CNT == 4)
                {

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if (((digital_output_RD ^ digital_output_wr) & 0x00000fffff) == 0)
                    {
                        panel42.BackColor = Color.LimeGreen;
                        label58.Text = "PASSED";
                    }
                    else
                    {
                        panel42.BackColor = Color.Red;
                        label58.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel37.BackColor = Color.LimeGreen;
                        label53.Text = "PASSED";
                    }
                    else
                    {
                        panel37.BackColor = Color.Red;
                        label53.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel32.BackColor = Color.LimeGreen;
                        label48.Text = "PASSED";
                    }
                    else
                    {
                        panel32.BackColor = Color.Red;
                        label48.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel27.BackColor = Color.LimeGreen;
                        label43.Text = "PASSED";
                    }
                    else
                    {
                        panel27.BackColor = Color.Red;
                        label43.Text = "FAILURE";
                    }


                    digital_output_wr = 0x8888888888;
    //                digital_output_wr = 0x8;
                    digital_input_wr = 0x7f;

                    analog_intput_wr[0] = 6000;
                    analog_intput_wr[1] = 3000;
                    analog_intput_wr[2] = 0;
                    analog_intput_wr[3] = 10000;

                    analog_output_wr[0] = 30000;
                    analog_output_wr[1] = 10000;
                    analog_output_wr[2] = 0;
                    analog_output_wr[3] = 50000;

                }
                else
                {
                    check_CNT = 0;

                    digital_output_ERR = (digital_output_ERR | (digital_output_RD ^ digital_output_wr)) & 0x00000fffff;
                    digital_input_ERR = digital_input_ERR | ((digital_input_RD ^ digital_input_wr) & 0x3f);
                    ADC_check1();
                    ADC_check2();
                    if (((digital_output_RD ^ digital_output_wr) & 0x00000fffff) == 0)
                    {
                        panel41.BackColor = Color.LimeGreen;
                        label57.Text = "PASSED";
                    }
                    else
                    {
                        panel41.BackColor = Color.Red;
                        label57.Text = "FAILURE";
                    }

                    if (((digital_input_RD ^ digital_input_wr) & 0x3f) == 0)
                    {
                        panel36.BackColor = Color.LimeGreen;
                        label52.Text = "PASSED";
                    }
                    else
                    {
                        panel36.BackColor = Color.Red;
                        label52.Text = "FAILURE";
                    }

                    if (analog_intput_ERR1 == 0)
                    {
                        panel31.BackColor = Color.LimeGreen;
                        label47.Text = "PASSED";
                    }
                    else
                    {
                        panel31.BackColor = Color.Red;
                        label47.Text = "FAILURE";
                    }

                    if (analog_output_ERR1 == 0)
                    {
                        panel26.BackColor = Color.LimeGreen;
                        label42.Text = "PASSED";
                    }
                    else
                    {
                        panel26.BackColor = Color.Red;
                        label42.Text = "FAILURE";
                    }
                }


            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
