import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule, RouterLink, ActivatedRoute } from '@angular/router';
import { CategoryService } from '../category.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-category-edit',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, RouterLink], 
  templateUrl: './category-edit.html', // Bạn nhớ đổi tên file HTML nếu cần nhé
  styleUrl: './category-edit.css'
})
export class CategoryEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private route = inject(ActivatedRoute); // Inject ActivatedRoute để lấy ID từ URL
  private authService = inject(AuthService); 

  categoryId!: string; // Hoặc number tùy thuộc vào kiểu dữ liệu ID của bạn

  // Khởi tạo form trống tương tự như bên Create
  categoryForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    isActived: [true]
  });

  // Getter để dùng ngoài HTML
  get f() { return this.categoryForm.controls; }

  ngOnInit() {
    // 1. Lấy ID từ Route Parameter (Ví dụ định nghĩa route là: /categories/edit/:id)
    this.categoryId = this.route.snapshot.paramMap.get('id') || '';

    if (this.categoryId) {
      this.loadCategoryDetails();
    }
  }

  private loadCategoryDetails() {
    // 2. Gọi service lấy chi tiết Category để fill vào form
    this.categoryService.getCategoryById(this.categoryId).subscribe({
      next: (category) => {
        // Đổ dữ liệu cũ vào form
        this.categoryForm.patchValue({
          name: category.name,
          description: category.description,
          isActived: category.isActived
        });
      },
      error: (err) => {
        console.error('Failed to load category details:', err);
        // Nếu lỗi (không tìm thấy ID), có thể điều hướng user quay lại danh sách
        this.router.navigate(['/categories']);
      }
    });
  }

  onSubmit() {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    // 3. Lấy tên người dùng hiện tại để ghi nhận ai là người cập nhật
    const loggedInUser = this.authService.identityClaims?.['name'] || 'User';

    // 4. Gom dữ liệu form và thêm trường audit 'updatedBy'
    const updatedCategory = {
      ...this.categoryForm.value,
      id: this.categoryId, // Đính kèm ID để API biết cần update bản ghi nào
      updatedBy: loggedInUser
    };
    
    // 5. Gọi service để cập nhật dữ liệu
    this.categoryService.updateCategory(this.categoryId, updatedCategory).subscribe({
      next: () => {
        // Trở về trang danh sách sau khi update thành công
        this.router.navigate(['/categories']);
      },
      error: (err) => {
        console.error('Failed to update category:', err);
      }
    });
  }
}