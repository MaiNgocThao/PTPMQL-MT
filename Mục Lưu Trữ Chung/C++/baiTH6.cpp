#include <iostream>
#include <string>
#include <limits>
using namespace std;

struct SinhVien
{
    string maSV;
    string HoTen;
    int NamSinh;
    double diemTK;
};

int main()
{
    int n;
    cout << "Nhap so luong sinh vien: ";
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

        cout << "Diem tong ket: ";
        cin >> ds[i].diemTK;
    }

    
    bool found = false;

    cout << "\n===== KET QUA TIM KIEM TEN 'DOANH' =====\n";

    for (int i = 0; i < n; i++)
    {
        if (ds[i].HoTen.find("Doanh") != string::npos)
        {
            cout << "MaSV: " << ds[i].maSV << "\n";
            cout << "Ho ten: " << ds[i].HoTen << "\n";
            cout << "Nam sinh: " << ds[i].NamSinh << "\n";
            cout << "Diem TK: " << ds[i].diemTK << "\n";
            cout << "----------------------\n";

            found = true;
        }
    }

    if (!found)
        cout << "Khong tim thay sinh vien ten 'Doanh'\n";

    return 0;
}