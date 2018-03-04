using System.Collections;
using System.Collections.Generic;

public class Player {
  private int userId;
  private string username;
  private int classType;
  public Player (int userId, string username) {
    this.userId = userId;
    this.username = username;
    classType = -1;
  }
  public void setUsername (string username) {
    this.username = username;
  }
  public string getUsername () {
    return this.username;
  }
  public void setClassType (int classType) {
    this.classType = classType;
  }
  public int getClassType () {
    return this.classType;
  }
  public int getUserId () {
    return this.userId;
  }

}
