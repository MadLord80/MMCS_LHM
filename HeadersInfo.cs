using System;
using System.Collections;

namespace MMCS_LHM
{
    class HeadersInfo
    {
        public readonly int NUMBER_OF_MODULES_OFFSET = 18;
        public readonly int BLOCK_SIZE = 2048;
        public readonly int MII_SIZE = 64;
        public readonly int MMI_SIZE = 256;

        public byte[] hcurrent = new byte[0];
        public byte[] hdonor = new byte[0];
        public byte[] hnew = new byte[0];
        public int[] changed_id = new int[0];

        public Object[] models = {
            new Object[]{"C-01 - C-02", "NR261EM_CTR.MLD.ORG"},
            new Object[]{"E-01 - E-06", "NR261EM.MLD.ORG"},
            new Object[]{"N-01 - N-05", "NR261UM.MLD.ORG"},
            new Object[]{"P-01 - P-02", "NR261EM_PUG.MLD.ORG"},
            new Object[]{"R-01", "NR261EMR.MLD.ORG"},
            new Object[]{"R-02 - R-03", "NR261RM.MLD.ORG"},
            new Object[]{"J-01", "NR261JM.MLD.ORG"},
            new Object[]{"J-02 - J-04", "NR261JM7.MLD.ORG"},
            new Object[]{"J-05 - J-06", "NR261J11.MLD.ORG"}
        };

        private help_functions func = new help_functions();

        public HeadersInfo()
        {
        }

        public void set_hcurrent(byte[] bytes)
        {
            Array.Resize(ref this.hcurrent, bytes.Length);
            Array.Resize(ref this.hnew, bytes.Length);
            this.hcurrent = bytes;
            this.hnew = bytes;
            this.changed_id = new int[0];
        }

        public void set_hdonor(byte[] bytes)
        {
            Array.Resize(ref this.hdonor, bytes.Length);
            this.hdonor = bytes;
        }

        public byte[] get_MII(byte[] bytes, int id)
        {
            byte[] mii = new byte[this.MII_SIZE];
            int mii_offset = 20 + id * this.MII_SIZE;
            Array.Copy(bytes, mii_offset, mii, 0, this.MII_SIZE);
            return mii;
        }

        public byte[] get_MMI(byte[] bytes, int id)
        {
            byte[] mmi = new byte[this.MMI_SIZE];
            int mmi_offset = 20 + id * this.MII_SIZE + id * this.MMI_SIZE;
            Array.Copy(bytes, mmi_offset, mmi, 0, this.MMI_SIZE);
            return mmi;
        }

        public void change_MII(int current_id, int donor_id)
        {
            int cmii_offset = 20 + current_id * this.MII_SIZE;
            int dmii_offset = 20 + donor_id * this.MII_SIZE;
            Array.Copy(this.hdonor, dmii_offset, this.hnew, cmii_offset, this.MII_SIZE);
            set_changed_id(current_id);
        }

        public void change_MIIName(int current_id, string donor_name)
        {
            int cmii_offset = 20 + current_id * this.MII_SIZE + 4;
            byte[] bname = this.func.StringToByteArray(donor_name, 52);
            Array.Copy(bname, 0, this.hnew, cmii_offset, 52);
            set_changed_id(current_id);
        }

        public void change_MIIVersion(int id, string ver)
        {
            if (ver.Length == 0 || ver.Length > 8) return;
            if (ver.Length < 8)
            {
                int add_length = 8 - ver.Length;
                for (int i = 0; i < add_length; i++) ver = "0" + ver;
            }
            int mii_ver_offset = 20 + id * this.MII_SIZE + 56;
            
            byte[] new_ver = new byte[8];
            for (int i = 0; i < 8; i++) {
                new_ver[i] = Convert.ToByte(ver[i]);
            }
            Array.Copy(new_ver, 0, this.hnew, mii_ver_offset, 8);
            set_changed_id(id);
        }

        public void add_MII(int current_id, int donor_id)
        {
            //update Number of Accommodated Modules
            int old_nam = this.hnew[17];
            this.hnew[17]++;

            //prepare arrays
            byte[] htemp = new byte[this.hnew.Length];
            Array.Copy(this.hnew, htemp, this.hnew.Length);
            bool header_resize = (this.hnew.Length < 20 + this.hnew[17] * this.MII_SIZE + this.hnew[17] * this.MMI_SIZE) ? true : false;
            if (header_resize)
            {
                Array.Resize(ref this.hnew, this.hnew.Length + this.BLOCK_SIZE);
                for (int i = 0; i < this.hnew[17]; i++) set_changed_id(i);
            }
            Array.Clear(this.hnew, 0, this.hnew.Length);

            int before_new_offset = 20 + current_id * this.MII_SIZE;
            Array.Copy(htemp, 0, this.hnew,0,before_new_offset);

            int new_mii_offset = 20 + donor_id * this.MII_SIZE;
            Array.Copy(this.hdonor, new_mii_offset, this.hnew, before_new_offset, this.MII_SIZE);

            Array.Copy(htemp, before_new_offset, this.hnew, before_new_offset + this.MII_SIZE, (this.hnew[17] - 1 - current_id) * this.MII_SIZE);

            int old_allmii_offset = 20 + (this.hnew[17] - 1) * this.MII_SIZE;
            int new_allmii_offset = 20 + this.hnew[17] * this.MII_SIZE;

            byte[] temp = new byte[4];
            for (int i = 0; i < old_nam; i++)
            {
                int mmi_offset = 20 + old_nam * this.MII_SIZE + i * this.MMI_SIZE;
                int new_mmi_offset = (i <= current_id) ? mmi_offset + this.MII_SIZE : mmi_offset + this.MII_SIZE + this.MMI_SIZE;
                Array.Copy(htemp, mmi_offset + 68, temp, 0, 4);
                int old_start = this.func.ByteArrayLEToInt(temp);
                //new start offset
                if (header_resize) old_start += this.BLOCK_SIZE / 2;

                temp = this.func.IntToByteArrayLE(old_start);
                Array.Copy(htemp, mmi_offset, this.hnew, new_mmi_offset, 68);
                Array.Copy(temp, 0, this.hnew, new_mmi_offset + 68, 4);
                Array.Copy(htemp, mmi_offset + 72, this.hnew, new_mmi_offset + 72, this.MMI_SIZE - 72);
                if (current_id == i)
                {
                    set_changed_id(i);
                    Array.Copy(htemp, mmi_offset, this.hnew, new_mmi_offset + this.MMI_SIZE, 68);
                    Array.Copy(temp, 0, this.hnew, new_mmi_offset + this.MMI_SIZE + 68, 4);
                    Array.Copy(htemp, mmi_offset + 72, this.hnew, new_mmi_offset + this.MMI_SIZE + 72, this.MMI_SIZE - 72);
                }
            }
        }

        public void add_MIIName(int current_id, string donor_name)
        {
            //update Number of Accommodated Modules
            int old_nam = this.hnew[17];
            this.hnew[17]++;

            //prepare arrays
            byte[] htemp = new byte[this.hnew.Length];
            Array.Copy(this.hnew, htemp, this.hnew.Length);
            bool header_resize = (this.hnew.Length < 20 + this.hnew[17] * this.MII_SIZE + this.hnew[17] * this.MMI_SIZE) ? true : false;
            if (header_resize)
            {
                Array.Resize(ref this.hnew, this.hnew.Length + this.BLOCK_SIZE);
                for (int i = 0; i < this.hnew[17]; i++) set_changed_id(i);
            }
            Array.Clear(this.hnew, 0, this.hnew.Length);

            int before_new_offset = 20 + current_id * this.MII_SIZE + 4;
            Array.Copy(htemp, 0, this.hnew, 0, before_new_offset);

            byte[] bname = this.func.StringToByteArray(donor_name, 52);
            Array.Copy(bname, 0, this.hnew, before_new_offset, 52);
            Array.Copy(htemp, before_new_offset + 52, this.hnew, before_new_offset + 52, this.MII_SIZE - 52);

            Array.Copy(htemp, before_new_offset, this.hnew, before_new_offset + this.MII_SIZE, (this.hnew[17] - 1 - current_id) * this.MII_SIZE);

            int old_allmii_offset = 20 + (this.hnew[17] - 1) * this.MII_SIZE;
            int new_allmii_offset = 20 + this.hnew[17] * this.MII_SIZE;

            byte[] temp = new byte[4];
            for (int i = 0; i < old_nam; i++)
            {
                int mmi_offset = 20 + old_nam * this.MII_SIZE + i * this.MMI_SIZE;
                int new_mmi_offset = (i <= current_id) ? mmi_offset + this.MII_SIZE : mmi_offset + this.MII_SIZE + this.MMI_SIZE;
                Array.Copy(htemp, mmi_offset + 68, temp, 0, 4);
                int old_start = this.func.ByteArrayLEToInt(temp);
                //new start offset
                if (header_resize) old_start += this.BLOCK_SIZE / 2;

                temp = this.func.IntToByteArrayLE(old_start);
                Array.Copy(htemp, mmi_offset, this.hnew, new_mmi_offset, 68);
                Array.Copy(temp, 0, this.hnew, new_mmi_offset + 68, 4);
                Array.Copy(htemp, mmi_offset + 72, this.hnew, new_mmi_offset + 72, this.MMI_SIZE - 72);
                if (current_id == i)
                {
                    set_changed_id(i);
                    Array.Copy(htemp, mmi_offset, this.hnew, new_mmi_offset + this.MMI_SIZE, 68);
                    Array.Copy(temp, 0, this.hnew, new_mmi_offset + this.MMI_SIZE + 68, 4);
                    Array.Copy(htemp, mmi_offset + 72, this.hnew, new_mmi_offset + this.MMI_SIZE + 72, this.MMI_SIZE - 72);
                }
            }
        }

        private void set_changed_id(int id)
        {
            if (Array.IndexOf(this.changed_id, id) == -1)
            {
                Array.Resize(ref this.changed_id, this.changed_id.Length + 1);
                this.changed_id[this.changed_id.Length - 1] = id;
            }
        }
    }
}
