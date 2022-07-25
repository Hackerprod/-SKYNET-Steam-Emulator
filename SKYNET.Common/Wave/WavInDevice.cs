
namespace SKYNET.Wave
{
    public class WavInDevice
    {
        private int    m_Index    = 0;
        private string m_Name     = "";
        private int    m_Channels = 1;

        internal WavInDevice(int index,string name,int channels)
        {
            m_Index    = index;
            m_Name     = name;
            m_Channels = channels;
        }

        public string Name
        {
            get{ return m_Name; }
        }

        public int Channels
        {
            get{ return m_Channels; }
        }

        internal int Index
        {
            get{ return m_Index; }
        }
    }
}
