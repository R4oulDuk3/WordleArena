locals {
  project = "wordle-arena-418422"
  network_name = "prod"
}

data "google_compute_network" "default" {
  name = "prod"
}

resource "google_compute_global_address" "private_ip_address" {
  
  name          = "cloud-sql-private-ip"
  purpose       = "VPC_PEERING"
  address_type  = "INTERNAL"
  prefix_length = 16
  network       = data.google_compute_network.default.name
}

resource "google_service_networking_connection" "private_vpc_connection" {
  network                 = data.google_compute_network.default.name
  service                 = "servicenetworking.googleapis.com"
  reserved_peering_ranges = [google_compute_global_address.private_ip_address.name]
}

resource "google_sql_database_instance" "arena" {
  name             = "arena-db"
  database_version = "POSTGRES_15"
  region           = "europe-central2"

  settings {
    tier = "db-f1-micro"

    ip_configuration {
      ipv4_enabled        = false
      private_network     = data.google_compute_network.default.id
    }
  }
  
}

resource "google_sql_database" "arena" {
  name     = "arena"
  instance = google_sql_database_instance.arena.name
}

resource "google_sql_user" "arena" {
  name     = "arena"
  instance = google_sql_database_instance.arena.name
  password = "arena"
}

resource "google_project_iam_member" "db_admin" {
  member  = "serviceAccount:wordle-arena@wordle-arena-418422.iam.gserviceaccount.com"
  project = local.project
  role    = "roles/cloudsql.admin"
}

#wordle-arena@wordle-arena-418422.iam.gserviceaccount.com