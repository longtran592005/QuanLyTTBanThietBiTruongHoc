using System;
using DTO;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium category editor with top-aligned labels, inline validation, and unsaved-changes guard.
    /// </summary>
    public class CategoryEditorForm : PremiumEditorForm
    {
        public Category Category { get; private set; }

        private System.Windows.Forms.TextBox _nameField;
        private System.Windows.Forms.TextBox _descField;

        public CategoryEditorForm(Category category = null)
        {
            Category = category ?? new Category();
            Text = Category.CategoryId == 0 ? "Thêm danh mục mới" : "Sửa danh mục";
            Width = 480;
            Height = 340;

            InitializePremiumLayout();
            _nameField = AddTextField("Tên danh mục", required: true, row: 0, column: 0);
            _descField = AddTextField("Mô tả", multiline: true, row: 1, column: 0);

            if (category != null)
            {
                _nameField.Text = category.CategoryName;
                _descField.Text = category.Description;
            }
        }

        protected override string GetFormIcon() => "📂";
        protected override string GetPrimaryButtonText() => Category.CategoryId == 0 ? "Tạo mới" : "Cập nhật";

        protected override bool OnSave()
        {
            var name = ValidateRequired(_nameField, "Tên danh mục");
            if (name == null) return false;

            Category.CategoryName = name;
            Category.Description = _descField.Text.Trim();
            return true;
        }
    }
}