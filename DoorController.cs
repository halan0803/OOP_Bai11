/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Thiết bị mô phỏng mở khóa cửa thoát hiểm
    public sealed class DoorController
    {
        public bool IsUnlocked { get; private set; }

        public void Unlock()
        {
            IsUnlocked = true;
            Console.WriteLine("[DOOR] Cua thoat hiem da mo khoa.");
        }
    }
}