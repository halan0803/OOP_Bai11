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
        // Chỉ dùng để giả lập thiết bị lỗi khi kiểm thử.
        private readonly bool simulateFailure;

        public DoorController(bool simulateFailure = false)
        {
            this.simulateFailure = simulateFailure;
        }
        public bool IsUnlocked { get; private set; }

        public void Unlock()
        {
            if (simulateFailure)
            {
                throw new InvalidOperationException(
                    "Mo khoa cua that bai (mo phong).");
            }

            IsUnlocked = true;

            Console.WriteLine(
                "[DOOR] Cua thoat hiem da mo khoa.");
        }
    }
}