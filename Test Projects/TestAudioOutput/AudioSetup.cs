using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestAudioOutput {
    public partial class AudioSetup : Form {
        private AudioData _audioData;

        public AudioSetup(AudioData audioData) {
            InitializeComponent();
            _audioData = audioData;
            textBoxFileName.Text = _audioData.FilePath;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _audioData.FilePath = textBoxFileName.Text;
        }
    }
}
