terraform {
  backend "gcs" {
    bucket  = "wordle-arena-tf-state-prod"
    prefix  = "gke"
  }
}
