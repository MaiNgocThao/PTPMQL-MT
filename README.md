# PTPMQL-MT
# Bài 1
1. Lệnh tạo MVC mới : dotnet new mvc -o Tên Project
2. Xem lịch sử chỉnh sửa git log --oneline
3. Tạo nhánh trong github : git branch + Tên nhánh
    3.1 Chuyển nhánh : git checkout + Tên nhánh
    3.2 Tạo và chuyển nhánh trong 1 : git checkout -b + Tên
    3.3 Xóa nhánh : git branch -d Tên
    3.4 Muốn đưa code vào main : git switch main
                                git merge develop
4. Lệnh reset :
    4.1 git reset --soft HEAD~1 : Bỏ commit nhưng giữ lại code
    4.2 git reset --hard HEAD~1
5. Git remote
    5.1 git remote -v : Kiểm tra remote
    5.2 Thêm git git remote add origin + Link
    Đổi tên branch : git branch -M main
    Push lần đầu git push -u origin main
6.Lệnh tạo Controller mới : ac_controller

7. Cấu trúc pattern: "{controller=Home}/{action=Index}
