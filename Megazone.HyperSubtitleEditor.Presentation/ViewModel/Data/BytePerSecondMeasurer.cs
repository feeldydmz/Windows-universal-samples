using System;
using System.Windows.Threading;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class BytePerSecondMeasurer : ViewModelBase
    {
        public static BytePerSecondMeasurer Instance = new BytePerSecondMeasurer();
        private readonly object _lockerForDownload = new object();
        private readonly object _lockerForUpload = new object();
        private double _bytesForDownload;
        private double _bytesForUpload;
        private DateTime _dateTimeForDownload;
        private DateTime _dateTimeForUpload;
        private string _displayDownloadSpeed;
        private string _displayUploadSpeed;

        private BytePerSecondMeasurer()
        {
            var dt = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dt.Tick += DispatcherTimer_Tick;
            dt.Start();
            _dateTimeForDownload = DateTime.Now;
            _dateTimeForUpload = DateTime.Now;
        }

        public string DisplayDownloadSpeed
        {
            get => _displayDownloadSpeed;
            set => Set(ref _displayDownloadSpeed, value);
        }

        public string DisplayUploadSpeed
        {
            get => _displayUploadSpeed;
            set => Set(ref _displayUploadSpeed, value);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            double bytePerSecForDownload;
            double bytePerSecForUpload;
            lock (_lockerForDownload)
            {
                var bytesForDownload = _bytesForDownload;
                var now = DateTime.Now;

                bytePerSecForDownload = bytesForDownload / (now - _dateTimeForDownload).TotalSeconds;

                _bytesForDownload = 0;
                _dateTimeForDownload = now;
            }

            lock (_lockerForUpload)
            {
                var bytesForUpload = _bytesForUpload;
                var now = DateTime.Now;

                bytePerSecForUpload = bytesForUpload / (now - _dateTimeForUpload).TotalSeconds;

                _bytesForUpload = 0;
                _dateTimeForUpload = now;
            }

            DisplayDownloadSpeed = SetDisplaySpeed(bytePerSecForDownload);
            DisplayUploadSpeed = SetDisplaySpeed(bytePerSecForUpload);
        }

        private string SetDisplaySpeed(double bytePerSecond)
        {
            if (bytePerSecond < 1024)
                return $"{Math.Round(bytePerSecond, 2)} B/s";
            var kbps = bytePerSecond / 1024;
            if (kbps < 1024)
                return $"{Math.Round(kbps, 2)} KB/s";

            var mbps = kbps / 1024;
            if (mbps < 1024)
                return $"{Math.Round(mbps, 2)} MB/s";

            var gbps = mbps / 1024;
            return $"{Math.Round(gbps, 2)} GB/s";
        }

        public void Download(double bytes)
        {
            lock (_lockerForDownload)
            {
                _bytesForDownload += bytes;
            }
        }

        public void Upload(double bytes)
        {
            lock (_lockerForUpload)
            {
                _bytesForUpload += bytes;
            }
        }
    }
}