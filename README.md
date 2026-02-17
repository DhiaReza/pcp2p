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
p2p = avg FPS / MSRP
<p></p>
<p>where :</p>
p2p = price to performance

### Todo
1. Compile GPU hardware from Nvidia, AMD, and Intel dating back from 2015 (GTX 1000)