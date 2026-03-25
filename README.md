# pcp2p
<p>A repo to hold personal project about PC price to performance comparison website. The website offers price to performance ratio calculation for users without hassle of collecting those info themselves. They will also be able to
use their own price so that they can judge whether the device is worth it or not at that price.</p>

## Requirements :
1. Hardware catalogue : browse a list of CPUs and GPUs with their specs (TDP, Clock speed, Cored/threads, and price)
2. Dynamic Value Calculation : Calculate price to performance ration based on user defined price
3. Search and Filter : Users can search for a specific GPU or filter based on brand or something else
4. Data Visualization : Charts for comparing value from selected hardware

## Tech Stack :
1. Backend : ASP.NET MVC
2. Frontend : Bootstrap 5, Chart.js for charts
3. Database : SQL

## Price to Performance Ratio :
<p>The price to performance ratio will be calculated using : </p>
<p></p>
p2p = avg FPS / MSRP (or user defined price)
<p></p>
<p>where :</p>
p2p = price to performance

### Todo

🟢 The User Flow Map
Landing: Hero section with a clear promise + Quick Filters.
Selection: User selects Category (GPU/CPU) → Selects Budget/Use Case.
Discovery: List of top "Value Picks" based on your Price-to-Performance algorithm.
Comparison Mode: User clicks two items to open the side-by-side view.
Deep Dive: Detailed breakdown with charts and a final "Verdict."
🖥️ Page 1: The Landing Page (Home)
Goal: Immediate value proposition and quick filtering without clutter.

Header
Logo (Left): ValueChip ⚡
Nav Links: GPU Guide | CPU Guide | Methodology | About
CTA Button: "Start Comparison"
Hero Section (Center Stage)
Headline: "Stop Overpaying. Find the Best Price-to-Performance Ratio."
Sub-headline: Compare real-world gaming FPS and overall processing power against current market prices.
Primary Action Bar (The Filter): A horizontal bar with three distinct tabs:
🔍 Find a GPU (Default)
🧠 Find a CPU
⚖️ Compare Two Items
Quick Filters (Dropdowns next to the tabs):
Budget: 
0
−
0−500 | 
500
−
500−1000 | $1000+
Use Case: 🎮 Gaming | 💻 Streaming | 🎬 Rendering
Featured Section: "Today's Best Value Picks"
A carousel or grid showing 3 cards based on the current market.
Card Example: RTX 4060 Ti
Tag: 🔥 Best Value Under $350
Metric: 120 FPS Avg (Raster) | Score: 9.8/10
Price Trend: 📉 -5% this week
🖥️ Page 2: The Selection List (Discovery)
Goal: Let users browse options based on their specific constraints before comparing.

Sidebar Filters (Left):
Max Price Slider (
0
−
0−2000)
VRAM Size (8GB, 12GB, 16GB+)
Release Year
Architecture Series (e.g., RTX 40-series only)
Main Content Area:
List of products sorted by "Best Value Score" (Your proprietary metric).
Each row shows: Image | Name | Price | Performance/Price Ratio (Highlighted in Green if high, Red if low).
Hover effect reveals a small bar chart comparing it to the previous generation.
🖥️ Page 3: The Comparison Tool (The Core Feature)
Goal: Visualize the difference clearly and justify the price gap.
Accessed via "Compare Two Items" button or clicking "Add to Compare" on two different products.

Layout Structure: Split Screen
The screen is divided into two vertical columns (Left vs. Right). A toggle switch at the top allows users to swap items instantly.


