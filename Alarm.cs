/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Thiết bị mô phỏng còi báo động
    public sealed class Alarm
    {
        public bool IsActive { get; private set; }

        public void Activate()
        {
            IsActive = true;
            Console.WriteLine("[ALARM] Bao dong da bat.");
        }
    }
}