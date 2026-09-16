#include <iostream>
#include <string>
#include <algorithm>
#include <limits>
using namespace std;

struct SinhVien
{
    string maSV;
    string HoTen;
    int NamSinh;
    string GioiTinh;
    double diemTK;
};

// ===== HIEN THI DANH SACH =====
void hienThiDanhSach(const SinhVien ds[], int n)
{
    cout << "\n===== DANH SACH SINH VIEN =====\n";
    cout << "STT\tMaSV\tHoTen\t\tNamSinh\tGioiTinh\tDiemTK\n";

    for (int i = 0; i < n; i++)
    {
        cout << i + 1 << "\t"
             << ds[i].maSV << "\t"
             << ds[i].HoTen << "\t"
             << ds[i].NamSinh << "\t"
             << ds[i].GioiTinh << "\t\t"
             << ds[i].diemTK << "\n";
    }
}

// ===== TIM KIEM TUAN TU =====
int timkiemTuanTu(const SinhVien a[], int n, const string &x)
{
    for (int i = 0; i < n; i++)
    {
        if (a[i].maSV == x)
            return i;
    }
    return -1;
}

// ===== TIM KIEM NHI PHAN =====
int timkiemNhiPhan(const SinhVien a[], int left, int right, const string &x)
{
    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (a[mid].maSV == x)
            return mid;
        else if (a[mid].maSV < x)
            left = mid + 1;
        else
            right = mid - 1;
    }
    return -1;
}

// ===== TIM THEO TEN "DOANH" =====
void timTheoTenDoanh(const SinhVien ds[], int n)
{
    bool found = false;

    for (int i = 0; i < n; i++)
    {
        if (ds[i].HoTen.find("Doanh") != string::npos)
        {
            if (!found)
            {
                cout << "\n===== SINH VIEN TEN 'DOANH' =====\n";
                cout << "MaSV\tHoTen\t\tNamSinh\tDiemTK\n";
            }

            cout << ds[i].maSV << "\t"
                 << ds[i].HoTen << "\t"
                 << ds[i].NamSinh << "\t"
                 << ds[i].diemTK << "\n";

            found = true;
        }
    }

    if (!found)
        cout << "Khong tim thay sinh vien ten 'Doanh'\n";
}

int main()
{
    int n;
    cout << "Nhap so luong sinh vien (<=100): ";
    cin >> n;

    if (n <= 0 || n > 100)
    {
        cout << "So luong khong hop le!\n";
        return 0;
    }

    SinhVien ds[100];

    // ===== NHAP DANH SACH =====
    for (int i = 0; i < n; i++)
    {
        cout << "\nSinh vien thu " << i + 1 << ":\n";

        cout << "Ma SV: ";
        cin >> ds[i].maSV;

        cin.ignore(numeric_limits<streamsize>::max(), '\n');

        cout << "Ho ten: ";
        getline(cin, ds[i].HoTen);

        cout << "Nam sinh: ";
        cin >> ds[i].NamSinh;

        cout << "Gioi tinh: ";
        cin >> ds[i].GioiTinh;

        cout << "Diem tong ket: ";
        cin >> ds[i].diemTK;
    }

    // ===== MENU =====
    int choice;
    string x;
    int pos;

    do
    {
        cout << "\n===== MENU =====\n";
        cout << "1. Hien thi danh sach\n";
        cout << "2. Tim kiem theo ma (tuan tu)\n";
        cout << "3. Tim kiem theo ma (nhi phan)\n";
        cout << "4. Tim sinh vien ten 'Doanh'\n";
        cout << "0. Thoat\n";
        cout << "Nhap lua chon: ";
        cin >> choice;

        switch (choice)
        {
        case 1:
            hienThiDanhSach(ds, n);
            break;

        case 2:
            cout << "Nhap ma SV can tim: ";
            cin >> x;
            pos = timkiemTuanTu(ds, n, x);

            if (pos != -1)
                cout << "Tim thay tai vi tri: " << pos + 1 << "\n";
            else
                cout << "Khong tim thay\n";
            break;

        case 3:
            sort(ds, ds + n, [](SinhVien a, SinhVien b)
                 { return a.maSV < b.maSV; });

            cout << "Nhap ma SV can tim: ";
            cin >> x;

            pos = timkiemNhiPhan(ds, 0, n - 1, x);

            if (pos != -1)
                cout << "Tim thay tai vi tri: " << pos + 1 << "\n";
            else
                cout << "Khong tim thay\n";
            break;

        case 4:
            timTheoTenDoanh(ds, n);
            break;

        case 0:
            cout << "Thoat chuong trinh.\n";
            break;

        default:
            cout << "Lua chon khong hop le!\n";
        }

    } while (choice != 0);

    return 0;
}
