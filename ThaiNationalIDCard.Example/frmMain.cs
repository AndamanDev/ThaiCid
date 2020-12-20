using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ThaiNationalIDCard.Example
{
    public partial class frmMain : Form
    {
        public static string dirParameter = Application.StartupPath + "\\cid.txt";
        private ThaiIDCard idcard;
        public frmMain()
        {
            InitializeComponent();
        }

        public void Log(string text = "")
        {
            if (txtBoxLog.InvokeRequired)
            {
                txtBoxLog.BeginInvoke(new MethodInvoker(delegate { txtBoxLog.AppendText(text); }));
            }
            else
            {
                txtBoxLog.AppendText(text);
            }
        }

        public void LogLine(string text = "")
        {
            if (txtBoxLog.InvokeRequired)
            {
                txtBoxLog.BeginInvoke(new MethodInvoker(delegate { txtBoxLog.AppendText(text + Environment.NewLine); }));
            }
            else
            {
                txtBoxLog.AppendText(text + Environment.NewLine);
            }
        }


        public void saveFile()
        {
            string Msg = lbl_cid.Text + ";" + lbl_th_prefix.Text + ";" + lbl_th_firstname.Text + ";" + lbl_th_lastname.Text + ";" + lbl_address.Text;

            // Save File to .txt  
            FileStream fParameter = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter, Encoding.GetEncoding(874));
            m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
            m_WriterParameter.Write(Msg);
            m_WriterParameter.Flush();
            m_WriterParameter.Close();
        }

        
        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                lbl_cid.Text = "Reading...";
                Refresh();
                Personal personal = idcard.readAll();
                if (personal != null)
                {
                    lbl_cid.Text = personal.Citizenid;
                    lbl_birthday.Text = personal.Birthday.ToString("dd/MM/yyyy");
                    lbl_sex.Text = personal.Sex;
                    lbl_th_prefix.Text = personal.Th_Prefix;
                    lbl_th_firstname.Text = personal.Th_Firstname;
                    lbl_th_lastname.Text = personal.Th_Lastname;
                    lbl_en_prefix.Text = personal.En_Prefix;
                    lbl_en_firstname.Text = personal.En_Firstname;
                    lbl_en_lastname.Text = personal.En_Lastname;
                    lbl_issue.Text = personal.Issue.ToString("dd/MM/yyyy");
                    lbl_expire.Text = personal.Expire.ToString("dd/MM/yyyy");
                    lbl_address.Text = personal.Address;
                    saveFile();

                    LogLine(personal.Address);
                    LogLine(personal.addrHouseNo); // บ้านเลขที่ 
                    LogLine(personal.addrVillageNo); // หมู่ที่
                    LogLine(personal.addrLane); // ซอย
                    LogLine(personal.addrRoad); // ถนน
                    LogLine(personal.addrTambol);
                    LogLine(personal.addrAmphur);
                    LogLine(personal.addrProvince);
                    LogLine(personal.Issuer);
                }
                else if (idcard.ErrorCode() > 0)
                {
                    MessageBox.Show(idcard.Error());
                }
                else
                {
                    MessageBox.Show("Catch all");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void photoProgress(int value, int maximum)
        {
            if (txtBoxLog.InvokeRequired)
            {
                if (PhotoProgressBar1.Maximum != maximum)
                    PhotoProgressBar1.BeginInvoke(new MethodInvoker(delegate { PhotoProgressBar1.Maximum = maximum; }));

                // fix progress bar sync.
                if (PhotoProgressBar1.Maximum > value)
                    PhotoProgressBar1.BeginInvoke(new MethodInvoker(delegate { PhotoProgressBar1.Value = value + 1; }));

                PhotoProgressBar1.BeginInvoke(new MethodInvoker(delegate { PhotoProgressBar1.Value = value; }));
            }
            else
            {
                if (PhotoProgressBar1.Maximum != maximum)
                    PhotoProgressBar1.Maximum = maximum;

                // fix progress bar sync.
                if (PhotoProgressBar1.Maximum > value)
                    PhotoProgressBar1.Value = value + 1;

                PhotoProgressBar1.Value = value;
            }

        }


        public void CardInserted(Personal personal)
        {
            if (personal == null )
            {
                if(idcard.ErrorCode() > 0)
                {
                    MessageBox.Show(idcard.Error());
                }
                return;
            }
                
            lbl_cid.BeginInvoke(new MethodInvoker(delegate { lbl_cid.Text = personal.Citizenid; }));
            lbl_birthday.BeginInvoke(new MethodInvoker(delegate { lbl_birthday.Text = personal.Birthday.ToString("dd/MM/yyyy"); }));
            lbl_sex.BeginInvoke(new MethodInvoker(delegate { lbl_sex.Text = personal.Sex; }));
            lbl_th_prefix.BeginInvoke(new MethodInvoker(delegate { lbl_th_prefix.Text = personal.Th_Prefix; }));
            lbl_th_firstname.BeginInvoke(new MethodInvoker(delegate { lbl_th_firstname.Text = personal.Th_Firstname; }));
            lbl_th_lastname.BeginInvoke(new MethodInvoker(delegate { lbl_th_lastname.Text = personal.Th_Lastname; }));
            lbl_en_prefix.BeginInvoke(new MethodInvoker(delegate { lbl_en_prefix.Text = personal.En_Prefix; }));
            lbl_en_firstname.BeginInvoke(new MethodInvoker(delegate { lbl_en_firstname.Text = personal.En_Firstname; }));
            lbl_en_lastname.BeginInvoke(new MethodInvoker(delegate { lbl_en_lastname.Text = personal.En_Lastname; }));
            lbl_issue.BeginInvoke(new MethodInvoker(delegate { lbl_issue.Text = personal.Issue.ToString("dd/MM/yyyy"); }));
            lbl_expire.BeginInvoke(new MethodInvoker(delegate { lbl_expire.Text = personal.Expire.ToString("dd/MM/yyyy"); }));
            lbl_address.BeginInvoke(new MethodInvoker(delegate { lbl_address.Text = personal.Address; }));
            pictureBox1.BeginInvoke(new MethodInvoker(delegate { pictureBox1.Image = personal.PhotoBitmap; }));
            Bitmap pic1 = new Bitmap(personal.PhotoBitmap);
            personal.PhotoBitmap.Dispose();
            ConvertTo24bpp(pic1).Save(Application.StartupPath + "\\cid.jpg", ImageFormat.Jpeg);
            saveFile();

        }

        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }


        private void btnReadWithPhoto_Click_1(object sender, EventArgs e)
        {
            lbl_cid.Text = "Reading...";
            Refresh();
            idcard.eventPhotoProgress += new handlePhotoProgress(photoProgress);
            Personal personal = idcard.readAllPhoto();
            if (personal != null)
            {
                lbl_cid.Text = personal.Citizenid;
                lbl_birthday.Text = personal.Birthday.ToString("dd/MM/yyyy");
                lbl_sex.Text = personal.Sex;
                lbl_th_prefix.Text = personal.Th_Prefix;
                lbl_th_firstname.Text = personal.Th_Firstname;
                lbl_th_lastname.Text = personal.Th_Lastname;
                lbl_en_prefix.Text = personal.En_Prefix;
                lbl_en_firstname.Text = personal.En_Firstname;
                lbl_en_lastname.Text = personal.En_Lastname;
                lbl_issue.Text = personal.Issue.ToString("dd/MM/yyyy");
                lbl_expire.Text = personal.Expire.ToString("dd/MM/yyyy");
                lbl_address.Text = personal.Address;
                pictureBox1.Image = personal.PhotoBitmap;
            }
            else if (idcard.ErrorCode() > 0)
            {
                MessageBox.Show(idcard.Error());
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ThaiIDCard idcard = new ThaiIDCard();
            //lbLibVersion.Text = "LibThaiIDCard version: " + idcard.Version();
            string[] readers = idcard.GetReaders();
            foreach (string r in readers)
            {
                idcard.MonitorStart(r);
                idcard.eventCardInsertedWithPhoto += new handleCardInserted(CardInserted);
                idcard.eventPhotoProgress += new handlePhotoProgress(photoProgress);

            }
        }

        private void cbxReaderList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
