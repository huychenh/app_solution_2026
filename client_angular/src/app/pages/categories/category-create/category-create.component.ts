import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule, RouterLink } from '@angular/router';
import { CategoryService } from '../category.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-category-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, RouterLink], 
  templateUrl: './category-create.html',
  styleUrl: './category-create.css'
})
export class CategoryCreateComponent {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  // 2. Inject AuthService to extract current login session data
  private authService = inject(AuthService); 

  // Initialize an independent form group for creating a category
  categoryForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    isActived: [true] // Default status is set to Active
  });

  // Getter for easy access to form fields in the HTML template
  get f() { return this.categoryForm.controls; }

  onSubmit() {
    // Mark all fields as touched to display validation errors if the form is invalid
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    // 3. Extract the logged-in user's name from identity claims, fallback to 'User' if not found
    const loggedInUser = this.authService.identityClaims?.['name'] || 'User';

    // 4. Combine form values with the audit field 'createdBy'
    const newCategory = {
      ...this.categoryForm.value,
      createdBy: loggedInUser,
      updatedBy: loggedInUser
    };
    
    // 5. Call the service to save the new category data
    this.categoryService.addCategory(newCategory).subscribe({
      next: () => {
        // Navigate back to the category list view upon successful creation
        this.router.navigate(['/categories']);
      },
      error: (err) => {
        console.error('Failed to create new category:', err);
      }
    });
  }
}