import csv, requests

# 1. Download the WPI CSV
url = "https://msi.nga.mil/api/publications/download?type=view&key=16920959/SFH00000/UpdatedPub150.csv"
r = requests.get(url)
r.raise_for_status()
decoded = r.content.decode('utf-8', errors='ignore').splitlines()

# 2. Parse CSV
reader = csv.DictReader(decoded)
ports = []
for row in reader:
    try:
        lat = float(row['Latitude'])
        lon = float(row['Longitude'])
    except:
        continue
    ports.append((row['Main Port Name'], row['Country Code'], row['UN/LOCODE'], lat, lon))

# 3. Generate SQL
with open("global_ports_wpi.sql", "w", encoding="utf-8") as f:
    f.write("-- WPI-based global ports dataset\nUSE ShipTracker;\nGO\n")
    for name, country, unlocode, lat, lon in ports:
        name_s = name.replace("'", "''") if name else ''
        country_s = country.replace("'", "''") if country else ''
        unlocode_s = unlocode.replace("'", "''") if unlocode else ''
        sql = f"INSERT INTO Ports (PortName, Country, Latitude, Longitude, UNLOCODE) VALUES ('{name_s}', '{country_s}', {lat}, {lon}, '{unlocode_s}');\n"
        f.write(sql)
