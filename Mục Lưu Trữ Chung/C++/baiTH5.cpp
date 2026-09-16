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

int timkiemTuanTu(const SinhVien a[], int n, const string &x)
{
    for (int i = 0; i < n; i++)
    {
        if (a[i].maSV == x)
            return i;
    }
    return -1;
}

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

    int choice;
    string x;

    cout << "\nChon phuong thuc tim kiem:\n";
    cout << "1. Tim kiem tuan tu\n";
    cout << "2. Tim kiem nhi phan\n";
    cout << "Nhap lua chon: ";
    cin >> choice;

    cout << "Nhap ma sinh vien can tim: ";
    cin >> x;

    int pos = -1;

    if (choice == 1)
    {
        pos = timkiemTuanTu(ds, n, x);
    }
    else if (choice == 2)
    {
        
        sort(ds, ds + n, [](SinhVien a, SinhVien b)
             { return a.maSV < b.maSV; });

        pos = timkiemNhiPhan(ds, 0, n - 1, x);
    }
    else
    {
        cout << "Lua chon khong hop le!\n";
        return 0;
    }

    if (pos != -1)
        cout << "Tim thay tai vi tri: " << pos + 1 << "\n";
    else
        cout << "Khong tim thay sinh vien\n";

    return 0;
}