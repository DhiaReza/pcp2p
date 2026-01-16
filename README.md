pcp2p
A repo to hold personal project about PC price to performance comparison website. The website offers price to performance ratio calculation for users without hassle of collecting those info themselves. They will also be able to
use their own price so that they can judge whether the device is worth it or not at that price.

Requirements :
Hardware catalogue : browse a list of CPUs and GPUs with their specs (TDP, Clock speed, Cored/threads, and price)
Dynamic Value Calculation : Calculate price to performance ration based on user defined price
Search and Filter : Users can search for a specific GPU or filter based on brand or something else
Data Visualization : Charts for comparing value from selected hardware

Tech Stack :
Backend : ASP.NET MVC
Frontend : Bootstrap 5, Chart.js for charts
Database : SQL

Price to Performance Ratio :
The price to performance ratio will be calculated using :
p2p = avg FPS/price
where :
p2p = price to performance
