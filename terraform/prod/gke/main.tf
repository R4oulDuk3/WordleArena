locals {
  project = "wordle-arena-418422"
  network_name = "prod"
  cluster_name = "prod-1"
  region = "europe-central2"
}

resource "google_compute_network" "default" {
  name                    = local.network_name
  auto_create_subnetworks = "false"
  routing_mode = "REGIONAL"
}

resource "google_compute_subnetwork" "default" {
  depends_on    = [google_compute_network.default]
  name          = "${local.cluster_name}-subnet"
  project       = google_compute_network.default.project
  region        = local.region
  network       = google_compute_network.default.name
  ip_cidr_range = "10.0.0.0/24"
}

resource "google_container_cluster" "gke-cluster" {
  name     = local.cluster_name
  location = local.region
  initial_node_count = 1
  # More info on the VPC native cluster: https://cloud.google.com/kubernetes-engine/docs/how-to/standalone-neg#create_a-native_cluster
  networking_mode = "VPC_NATIVE"
  network    = google_compute_network.default.name
  subnetwork = google_compute_subnetwork.default.name
  # Disable the Google Cloud Logging service because you may overrun the Logging free tier allocation, and it may be expensive
  logging_service = "logging.googleapis.com/kubernetes"

  node_config {
    # More info on Spot VMs with GKE https://cloud.google.com/kubernetes-engine/docs/how-to/spot-vms#create_a_cluster_with_enabled
    spot = true
    machine_type = "e2-small"
    disk_size_gb = 10
    tags = ["${local.cluster_name}"]
    oauth_scopes = [
      "https://www.googleapis.com/auth/cloud-platform",
      "https://www.googleapis.com/auth/trace.append",
      "https://www.googleapis.com/auth/service.management.readonly",
      "https://www.googleapis.com/auth/monitoring",
      "https://www.googleapis.com/auth/devstorage.read_only",
      "https://www.googleapis.com/auth/servicecontrol",
    ]
    workload_metadata_config {
      mode          = "GKE_METADATA"
    }
  }

  deletion_protection = false

  addons_config {
    http_load_balancing {
      # This needs to be enabled for the NEG to be automatically created for the ingress gateway svc
      disabled = false
    }
  }

  workload_identity_config {
    workload_pool  = "${local.project}.svc.id.goog"
  }

  private_cluster_config {
    # Need to use private nodes for VPC-native GKE clusters
    enable_private_nodes = true
    # Allow private cluster Master to be accessible outside of the network
    enable_private_endpoint = false
    master_ipv4_cidr_block = "172.16.0.16/28"
  }

  ip_allocation_policy {
    cluster_ipv4_cidr_block = "5.0.0.0/16"
    services_ipv4_cidr_block = "5.1.0.0/16"
  }

  default_snat_status {
    disabled = true
  }

  master_authorized_networks_config {
    cidr_blocks {
      cidr_block = "0.0.0.0/0"
      display_name = "World"
    }
  }
}