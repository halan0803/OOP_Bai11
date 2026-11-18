/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Hợp đồng để mọi kênh thông báo nhận cùng kiểu dữ liệu
    public interface IAlertObserver
    {
        void Update(AlertEvent alertEvent);
    }
}