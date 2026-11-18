/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Giả định tình huống mô phỏng: tạm ngừng thông gió khi có khói
    // Trong thực tế, hành động này phải theo thiết kế phòng cháy chữa cháy
    public sealed class VentilationController
    {
        public bool IsStopped { get; private set; }

        public void Stop()
        {
            IsStopped = true;
            Console.WriteLine("[VENTILATION] Thong gio da tam dung.");
        }
    }
}