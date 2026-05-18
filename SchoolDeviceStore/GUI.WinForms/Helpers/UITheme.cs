using System.Drawing;

namespace GUI.WinForms
{
    public static class UITheme
    {
        // 1. Color Palette (Premium Utilitarian Minimalism)
        public static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255); // Pure White background
        public static readonly Color SurfaceColor = Color.FromArgb(255, 255, 255); // White for cards (blends in)
        public static readonly Color SubtleSurfaceColor = Color.FromArgb(249, 250, 251); // Very light gray for alternate rows/headers
        
        // Use Ceramic Blue as the Primary Brand Color
        public static readonly Color PrimaryColor = Color.FromArgb(37, 99, 235); // #2563EB (Tailwind Blue 600)
        public static readonly Color PrimaryHoverColor = Color.FromArgb(29, 78, 216); // #1D4ED8
        
        public static readonly Color TextPrimaryColor = Color.FromArgb(17, 17, 17); // #111111 Off-black for high contrast
        public static readonly Color TextSecondaryColor = Color.FromArgb(120, 119, 116); // #787774 Muted gray
        public static readonly Color BorderColor = Color.FromArgb(226, 232, 240); // #E2E8F0 Very subtle borders
        
        public static readonly Color SuccessColor = Color.FromArgb(16, 185, 129); // #10B981 Soft emerald
        public static readonly Color WarningColor = Color.FromArgb(245, 158, 11); // #F59E0B Soft amber
        public static readonly Color ErrorColor = Color.FromArgb(239, 68, 68); // #EF4444 Soft red
        
        // Sidebar styling (Warm Bone)
        public static readonly Color SidebarColor = Color.FromArgb(247, 246, 243); // #F7F6F3 (Notion-style sidebar)
        public static readonly Color SidebarHoverColor = Color.FromArgb(238, 238, 236); // Slightly darker for hover
        public static readonly Color SidebarActiveColor = Color.FromArgb(228, 227, 224); // Active state
        public static readonly Color SidebarMutedTextColor = Color.FromArgb(100, 100, 95);

        // Data grids and tables
        public static readonly Color TableAlternateColor = Color.FromArgb(251, 251, 250); // #FBFBFA
        public static readonly Color SelectionColor = Color.FromArgb(232, 242, 255); // #E8F2FF Soft blue selection
        public static readonly Color HoverFillColor = Color.FromArgb(243, 244, 246); // #F3F4F6

        // 2. Layout Tokens
        public const int SidebarWidth = 240; // Slightly wider for breathability
        public const int SpacingXs = 4;
        public const int SpacingSm = 8;
        public const int SpacingMd = 16;
        public const int SpacingLg = 24;
        public const int SpacingXl = 32;
        public const int BorderRadius = 8; // Softer 8px border radius

        // Typographic Hierarchy (Using Segoe UI for native Windows rendering, mimicking SF Pro/Inter)
        public static readonly Font BaseFont = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font BodyBoldFont = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font CaptionFont = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font SectionTitleFont = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font HeroFont = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point);
    }
}
