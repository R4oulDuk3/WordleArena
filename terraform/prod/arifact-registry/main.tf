locals {
  project = "wordle-arena-418422"
}

resource "google_artifact_registry_repository" "wordle_arena" {
  project = local.project
  location      = "europe-central2"
  repository_id = "main"
  description   = "Docker repository for Wordle Arena"
  format        = "DOCKER"

  docker_config {
    immutable_tags = true
  }
}