package laptrinhjava;
import java.util.Scanner;
import java.util.ArrayList;
import java.util.Comparator;

public class Student {
    public int id;
    public String name;
    public int age;
    public String grade;
    
    public int getId() {
        return id;
    }
    
    public void setId(int id) {
        this.id = id;
    }
    
    public String getName() {
        return name;
    }
    
    public void setName(String name) {
        this.name = name;
    }
    
    public int getAge() {
        return age;
    }
    
    public void setAge(int age) {
        this.age = age;
    }
    
    public String getGrade() {
        return grade;
    }
    
    public void setGrade(String grade) {
        this.grade = grade;
    }
    
    public void nhapThongtin(Scanner sc) {
        System.out.println("Nhap id:");
        id = sc.nextInt();
        sc.nextLine(); 
        System.out.println("Nhap ten:");
        name = sc.nextLine();
        System.out.println("Nhap tuoi:");
        age = sc.nextInt();
        sc.nextLine();
        System.out.println("Nhap bang cap:");
        grade = sc.nextLine();
    }
    
    public void hienThi() {
        System.out.println("Id:" + id);
        System.out.println("Ten:" + name);
        System.out.println("Tuoi:" + age);
        System.out.println("Bang cap:" + grade);
    }
    
    ArrayList<Student> sv = new ArrayList<>();
    
    public void addStudent(Student student) {
        sv.add(student);
    }
    
    public void removeStudent(Student student) {
        sv.remove(student);
    }
    
    public Student findsv(int id) {
        for (Student s : sv) {
            if (s.getId() == id) {
                return s;
            }
        }
        return null;
    }
    
    public void sortByName() {
        sv.sort(Comparator.comparing(Student::getName));
    }
}
